using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Web.Data.Models;
using static Web.WebConstants;

namespace Web.Tests.Data
{
    public static class UserTestData
    {
        public static User CustomeTestUser(string userId) =>
         new User() { Id = userId, UserName = "testUser",Applyed = false };

        public static User UserWithAdditionalData(string userId, int dataId) =>
            new User()
            {
                Id = userId,
                UserName = "testUser",
                UserDataId = dataId,               
            };
      
    }
}
