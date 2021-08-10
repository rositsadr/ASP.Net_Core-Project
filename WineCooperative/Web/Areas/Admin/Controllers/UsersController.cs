using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Services.Users;
using Web.Services.Users.Models;
using static Web.WebConstants;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AdministratorRole)]
    [Area(AreaName)]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;

        public UsersController(IUserService userService, UserManager<User> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        public IActionResult ApplyedUsers()
        {
            var users = userService.All()
                .Where(u => u.Applyed)
                .ToList();

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

            return RedirectToAction("ApplyedUsers");
        }

        public IActionResult AllMembers()
        {
            var usersId = userService.All().Select(u => u.Id);
            var members = new List<UserInfoServiceModel>();

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

            return RedirectToAction("AllMembers");
        }
    }
}
