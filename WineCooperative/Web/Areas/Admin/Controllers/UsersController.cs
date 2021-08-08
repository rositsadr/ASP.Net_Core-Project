using AutoMapper;
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
        private readonly IMapper mapper;

        public UsersController(IUserService userService, UserManager<User> userManager, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public IActionResult ApplyedUsers()
        {
            var users = userService.All()
                .Where(u => u.Applyed)
                .ToList();

            return View(users);
        }

        public IActionResult ApproveMember(string Id)
        {
            Task.Run(async () =>
            {
                var user = await userManager.FindByIdAsync(Id);
                await userManager.AddToRoleAsync(user, MemberRole);
            })
                .GetAwaiter()
                .GetResult();

            userService.NotApplyed(Id);

            return RedirectToAction("AllMembers");
        }

        public IActionResult DeclineMember(string Id)
        {
            userService.NotApplyed(Id);

            return RedirectToAction("AllMembers");
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
                    members.Add(mapper.Map<UserInfoServiceModel>(user));
                }
            })
                .GetAwaiter()
                .GetResult();
            }

            return View(members);
        }


    }
}
