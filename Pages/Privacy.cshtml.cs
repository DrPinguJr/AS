using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dit230680c_AS.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        // ✅ Reuse ValidateSessionAsync() inside the service
        private async Task<IActionResult> ValidateSessionAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userEmail = httpContext.Session.GetString("UserEmail");
                var sessionId = httpContext.Session.GetString("SessionId");

                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(sessionId))
                {
                    return new RedirectToPageResult("/Account/Login"); // Redirect if invalid
                }

                return null; // Session is valid
            }

            return new RedirectToPageResult("/Account/Login");
        }

        public void OnGet()
        {
        }
    }

}
