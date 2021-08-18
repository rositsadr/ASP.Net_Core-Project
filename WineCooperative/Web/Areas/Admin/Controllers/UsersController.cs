using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Services.Users;
using Web.Services.Users.Models;
using Web.Services.Manufacturers.Models;
using static Web.WebConstants;
using static Web.Areas.AreaConstants;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AdministratorRole)]
    [Area(AreaName)]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly IMemoryCache cache;

        public UsersController(IUserService userService, UserManager<User> userManager, IMemoryCache cache)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.cache = cache;
        }

        public IActionResult ApplyedUsers()
        {
            var users = this.cache.Get<List<UserInfoServiceModel>>(applyedCacheKey);

            if(users == null)
            {
                users = userService.All()
                    .Where(u => u.Applyed)
                    .ToList();

                cache.Set(applyedCacheKey, users);
            }

            return View(users);
        }

        public async Task<IActionResult> ApproveMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            if(await userManager.IsInRoleAsync(user,MemberRole))
            {
                return BadRequest($"The user is a member.");
            }

            await userManager.AddToRoleAsync(user, MemberRole);

            userService.NotApplyed(id);

            cache.Set<List<UserInfoServiceModel>>(membersCacheKey, null);
            cache.Set<List<UserInfoServiceModel>>(applyedCacheKey, null);

            return RedirectToAction("ApplyedUsers");
        }

        public IActionResult DeclineMember(string id)
        {
            if(!userService.UserExists(id))
            {
                return NotFound("User does not exists.");
            }

            if(!userService.UserApplyed(id))
            {
                return BadRequest("No application from that user found.");
            }

            userService.NotApplyed(id);

            cache.Set<List<UserInfoServiceModel>>(applyedCacheKey, null);

            return RedirectToAction("ApplyedUsers");
        }

        public IActionResult AllMembers()
        {
            var members = this.cache.Get<List<UserInfoServiceModel>>(membersCacheKey);

            if(members == null)
            {
                var usersId = userService.All().Select(u => u.Id);
                members = new List<UserInfoServiceModel>();

                foreach (var id in usersId)
                {
                    Task.Run(async () =>
                    {
                        var user = await userManager.FindByIdAsync(id);
                        if (await userManager.IsInRoleAsync(user, MemberRole))
                        {
                            var userMember = userService.GetUserWithData(id);
                            members.Add(userMember);
                        }
                    })
                    .GetAwaiter()
                    .GetResult();
                }

                cache.Set(membersCacheKey, members);
            }            

            return View(members);
        }

        public async Task<IActionResult> RemoveMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            if(!await userManager.IsInRoleAsync(user,MemberRole))
            {
                return BadRequest($"User is not a member.");
            }

            await userManager.RemoveFromRoleAsync(user, MemberRole);

            userService.ChangeAllUsersProductsToNotInStock(id);
            userService.ChangeAllServiceToNotAvailable(id);
            userService.ChangeAllManufacturersToNotFunctional(id);

            cache.Set<IEnumerable<ManufacturerServiceModel>>(manufacturersCacheKey, null);
            cache.Set<List<UserInfoServiceModel>>(membersCacheKey, null);
            
            this.TempData[SuccessMessageKey] = string.Format(SuccessfullyDeleted, "member");
            return RedirectToAction("AllMembers");
        }
    }
}
