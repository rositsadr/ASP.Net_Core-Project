using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Services.Users;
using static Web.Data.DataConstants;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService userService;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.userService = userService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} symbols long.", MinimumLength = NameMinLength)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} symbols long.", MinimumLength = NameMinLength)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            public string Street { get; set; }

            [Required]
            [StringLength(TownMaxLength, MinimumLength = TownMinLength)]
            [Display(Name = "Town")]
            public string TownName { get; set; }

            [Required]
            [StringLength(ZipCodeMaxLength, MinimumLength = ZipCodeMinLength)]
            public string ZipCode { get; set; }

            [Required]
            [StringLength(CountryMaxLength, MinimumLength = CountryMinLength)]
            [Display(Name = "Country")]
            public string CountryName { get; set; }
        }

        private void LoadAsync(User user)
        {
            var userId = user.Id;
            var personalData = userService.Edit(userId);

            if (User.GetId() == userId)
            {
                personalData = null;
            }

            Input = new InputModel
            {
                FirstName = personalData == null ? null : personalData.FirstName,
                LastName = personalData == null ? null : personalData.LastName,
                PhoneNumber = personalData == null ? null : personalData.PhoneNumber,
                Street = personalData == null ? null : personalData.Address.Street,
                TownName = personalData == null ? null : personalData.Address.TownName,
                ZipCode = personalData == null ? null : personalData.Address.ZipCode,
                CountryName = personalData == null ? null : personalData.Address.CountryName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.GetId();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var personalData = userService.Edit(userId);

            if(personalData == null)
            {
                userService.AddUserAdditionalInfo(userId, Input.FirstName, Input.LastName, Input.Street, Input.TownName, Input.ZipCode, Input.CountryName);
            }
            else
            {
                userService.ApplyChanges(userId, Input.FirstName, Input.LastName, Input.Street, Input.TownName, Input.ZipCode, Input.CountryName);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
