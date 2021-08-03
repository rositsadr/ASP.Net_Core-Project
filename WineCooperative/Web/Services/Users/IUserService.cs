namespace Web.Services.Users
{
   public interface IUserService
    {
        public bool UserIsManufacturer(string userId);

        public void AddUserAdditionalInfo(string userID, string firstName, string lastName, string street, string townName, string zipCode, string countryName);
    }
}
