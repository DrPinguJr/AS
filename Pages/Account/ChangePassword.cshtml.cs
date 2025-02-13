using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace dit230680c_AS.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ChangePasswordInputModel Input { get; set; } = new ChangePasswordInputModel();

        public string ErrorMessage { get; set; } = string.Empty;  // ✅ Added this property

        public class ChangePasswordInputModel
        {
            [Required]
            public string OldPassword { get; set; } = string.Empty;  // ✅ Prevents null reference errors

            [Required]
            public string NewPassword { get; set; } = string.Empty;  // ✅ Prevents null reference errors
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fix the errors below.";
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!result.Succeeded)
            {
                ErrorMessage = "Invalid password change request. Please check your old password.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage("/Account/Manage");
        }
    }
}
