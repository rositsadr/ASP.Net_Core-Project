using System.Collections.Generic;
using Web.Services.Users.Models;

namespace Web.Services.Users
{
   public interface IUserService
    {
        public bool UserIsManufacturer(string userId);

        public IEnumerable<UserInfoServiceModel> All();

        public void AddUserAdditionalInfo(string userID, string firstName, string lastName, string street, string townName, string zipCode, string countryName);

        public UserEditInfoServiceModel Edit(string userId);

        public bool ApplyChanges(string userId, string firstName, string lastName, string street, string townName, string zipCode, string countryName);

        public bool UserHasAdditionaInfo(string userId);

        public bool UserApplyed(string userId);

        public void ApplyForMember(string userId);

        public void NotApplyed(string userId);
    }
}
