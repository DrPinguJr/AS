using dit230680c_AS.Data;
using dit230680c_AS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dit230680c_AS.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // ✅ Validate user session before accessing service
        private async Task<IActionResult> ValidateSessionAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userEmail = httpContext.Session.GetString("UserEmail");
                var sessionId = httpContext.Session.GetString("SessionId");

                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(sessionId))
                {
                    return new RedirectToPageResult("/Account/Login"); // Redirect if invalid session
                }
                return null; // Session is valid
            }

            return new RedirectToPageResult("/Account/Login");
        }

        // ✅ Secure method that logs activities only for authenticated users
        public async Task<IActionResult> LogActivityAsync(string activity)
        {
            var validationResult = await ValidateSessionAsync();
            if (validationResult != null)
            {
                return validationResult; // Redirect if session is invalid
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var userId = _userManager.GetUserId(httpContext.User) ?? "Guest"; 
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            var auditLog = new AuditLog
            {
                UserId = userId,
                Activity = activity,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return null; // Log recorded successfully
        }

        // ✅ Ensure service methods are only accessed with valid sessions
        public async Task<IActionResult> SomeOtherServiceMethod()
        {
            var validationResult = await ValidateSessionAsync();
            if (validationResult != null)
            {
                return validationResult; // Redirect if session is invalid
            }

            // Continue execution of the service method...
            return null;
        }
    }
}
