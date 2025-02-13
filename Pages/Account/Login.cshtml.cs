using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Linq;
using dit230680c_AS.Data;
using dit230680c_AS.ViewModels;
using dit230680c_AS.Models;
using dit230680c_AS.Services;  // Add this to reference the GoogleReCaptchaService class and AuditLogService

namespace dit230680c_AS.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly GoogleReCaptchaService _reCaptchaService;  // Reference Google reCAPTCHA service
        private readonly AuditLogService _auditLogService;         // Reference AuditLog service
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          GoogleReCaptchaService reCaptchaService,  // Inject Google reCAPTCHA service
                          AuditLogService auditLogService,          // Inject AuditLog service
                          AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _reCaptchaService = reCaptchaService;
            _auditLogService = auditLogService;
            _context = context;
        }

        [BindProperty]
        public LoginView Input { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors in the form.";
                return Page();
            }

            // Verify reCAPTCHA token
            var reCaptchaToken = Request.Form["recaptcha-token"];
            if (string.IsNullOrEmpty(reCaptchaToken) || !await _reCaptchaService.VerifyCaptchaAsync(reCaptchaToken))
            {
                ErrorMessage = "reCAPTCHA validation failed. Please try again.";
                await _auditLogService.LogActivityAsync("Failed login attempt due to reCAPTCHA failure.");
                return Page();
            }

            // Authenticate using SignInManager for IdentityUser
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Generate unique session ID
                var sessionId = Guid.NewGuid().ToString();

                // Store user info in session
                HttpContext.Session.SetString("UserEmail", Input.Email);
                HttpContext.Session.SetString("SessionId", sessionId);

                // Retrieve the custom User model (not IdentityUser)
                var user = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
                if (user != null)
                {
                    // Update the user's CurrentSessionId in the database
                    user.CurrentSessionId = sessionId;
                    await _context.SaveChangesAsync(); // Save changes asynchronously
                }

                // Log successful login activity
                await _auditLogService.LogActivityAsync($"User successfully logged in: {Input.Email}");

                return RedirectToPage("/Home"); // Redirect to homepage after successful login
            }
            else if (result.IsLockedOut)
            {
                // Log failed login attempt due to account lockout
                await _auditLogService.LogActivityAsync("User account is locked due to multiple failed login attempts.");
                ErrorMessage = "Your account is locked due to multiple failed login attempts. Please try again later.";
            }
            else
            {
                // Log failed login attempt
                await _auditLogService.LogActivityAsync("Failed login attempt due to invalid credentials.");
                ErrorMessage = "Invalid login attempt. Please check your credentials.";
            }

            return Page();
        }
    }
}
