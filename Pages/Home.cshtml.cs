using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dit230680c_AS.Data;
using dit230680c_AS.Models;
using dit230680c_AS.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace dit230680c_AS.Pages
{
    public class HomeModel : PageModel
    {
        private readonly EncryptionHelper _encryptionHelper;
        private readonly AppDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Properties to hold user data
        public string UserEmail { get; set; }
        public string SessionId { get; set; }
        public string EncryptedCreditCardNo { get; set; }
        public string DecryptedCreditCardNo { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string DeliveryAddress { get; set; }
        public string AboutMe { get; set; }
        public string PhotoPath { get; set; }

        public HomeModel(AppDbContext context, SignInManager<IdentityUser> signInManager, EncryptionHelper encryptionHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _signInManager = signInManager;
            _encryptionHelper = encryptionHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<IActionResult> ValidateSessionAsync()
        {
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userEmail = _httpContextAccessor.HttpContext.Session.GetString("UserEmail");
                var sessionId = _httpContextAccessor.HttpContext.Session.GetString("SessionId");

                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(sessionId))
                {
                    return RedirectToPage("/Account/Login");
                }
                return null;
            }
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await ValidateSessionAsync();
            if (result != null) return result;

            UserEmail = HttpContext.Session.GetString("UserEmail");
            SessionId = HttpContext.Session.GetString("SessionId");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == UserEmail);
            if (user != null)
            {
                FullName = user.FullName;
                Gender = user.Gender;
                MobileNo = user.MobileNo;
                DeliveryAddress = user.DeliveryAddress;
                AboutMe = user.AboutMe;
                PhotoPath = user.PhotoPath;
                EncryptedCreditCardNo = user.CreditCardNo;

                if (!string.IsNullOrEmpty(EncryptedCreditCardNo) && !string.IsNullOrEmpty(user.EncryptionIV))
                {
                    try
                    {
                        DecryptedCreditCardNo = _encryptionHelper.Decrypt(EncryptedCreditCardNo, user.EncryptionIV);
                    }
                    catch
                    {
                        DecryptedCreditCardNo = "Decryption Failed";
                    }
                }
                else
                {
                    DecryptedCreditCardNo = "No Credit Card Info Available";
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToPage("/Account/Login");
        }
    }
}
