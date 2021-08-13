using Web.Data.Models;
using Web.Models;

namespace Web.Tests.Data
{
    public static class UserTestData
    {
        public static User TestUser(string userId) =>
         new User() { Id = userId, UserName = "testUser" };

        public static User UserWithAdditionalData(string userId, int dataId) =>
            new User()
            {
                Id = userId,
                UserName = "testUser",
                UserDataId = dataId,               
            };

    }
}
