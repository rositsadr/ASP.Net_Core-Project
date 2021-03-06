using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;
using static Web.WebConstants;

namespace Web.Tests.Data
{
    public static class UserTestData
    {
        public static User CustomeTestUser(string userId) =>
         new User() { Id = userId, UserName = "testUser",Applyed = false };

        public static User CustomeTestApplyedUser(string userId) =>
        new User() { Id = userId, UserName = "testUser", Applyed = true };

        public static User UserWithAdditionalData(string userId, int dataId) =>
            new User()
            {
                Id = userId,
                UserName = "testUser",
                UserDataId = dataId,               
            };

        public static IEnumerable<User> GetApplyedUsers(int count) =>
            Enumerable
            .Range(1, count)
            .Select(i => new User
            { 
                Id = i.ToString(),
                UserName = $"TestUser {i}",
                Applyed = true
            });

        public static IdentityRole GetMemberRole(string id) => new IdentityRole()
        {
            Id = id,
            Name = MemberRole,
            NormalizedName = MemberRole.ToUpper()
        };

        public static IdentityUserRole<string> GetUserRole(string userId, string roleId)
            => new IdentityUserRole<string> { RoleId = roleId, UserId = userId };

    }
}
