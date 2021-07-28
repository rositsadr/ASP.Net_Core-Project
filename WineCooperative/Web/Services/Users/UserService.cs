using System.Linq;
using Web.Data;

namespace Web.Services.Users
{
    public class UserService : IUserService
    {
        private readonly WineCooperativeDbContext data;

        public UserService(WineCooperativeDbContext data) => this.data = data;

        public bool UserIsManufacturer(string userId) => data.Manufacturers
            .Any(m => m.UserId == userId);
    }
}
