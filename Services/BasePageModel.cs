using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using dit230680c_AS.Data;
using dit230680c_AS.Models;

namespace dit230680c_AS.Services
{
    public class BasePageModel : PageModel
    {
        protected readonly AppDbContext _context;  // Changed from private to protected
        protected readonly SignInManager<IdentityUser> _signInManager;  // Changed from private to protected

        public BasePageModel(AppDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> ValidateSessionAsync()
        {
            var sessionId = HttpContext.Session.GetString("SessionId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login"); // No session data, redirect to login
            }

            // Check if the user exists in the Users table instead of Members
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null || user.CurrentSessionId != sessionId)
            {
                // Session mismatch detected, log out the user
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }

            return null;  // Session is valid, continue to the requested page
        }
    }
}
