using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dit230680c_AS.Models;
using dit230680c_AS.Services;
using dit230680c_AS.Data;
using dit230680c_AS.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dit230680c_AS.Pages.Account
{   
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EncryptionHelper _encryptionHelper;

        public RegisterModel(AppDbContext context, UserManager<IdentityUser> userManager, EncryptionHelper encryptionHelper)
        {
            _context = context;
            _userManager = userManager;
            _encryptionHelper = encryptionHelper;
        }

        [BindProperty]
        public RegisterView Input { get; set; }
        public string ErrorMessage { get; set; }
        public string DecryptedCreditCardNo { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors in the form.";
                return Page();
            }

            // Validate Password Strength on Server Side
            var passwordStrength = CheckPasswordStrength(Input.Password);
            if (passwordStrength < 4)
            {
                ErrorMessage = "Password is too weak. Please choose a stronger password.";
                return Page();
            }

            // Check for duplicate email in Users table (Fresh Farm Market)
            if (_context.Users.Any(u => u.Email == Input.Email))
            {
                ErrorMessage = "Email is already registered.";
                return Page();
            }

            // Encrypt CreditCardNo
            var (encryptedCreditCard, creditCardIV) = _encryptionHelper.Encrypt(Input.CreditCardNo);

            // Handle photo file upload (only JPG allowed)
            string photoPath = null;

            if (Input.Photo != null)
            {
                var allowedExtension = ".jpg";
                var fileExtension = Path.GetExtension(Input.Photo.FileName).ToLower();

                if (fileExtension != allowedExtension)
                {
                    ErrorMessage = "Only JPG files are allowed for photo upload.";
                    return Page();
                }

                // Generate unique file name to avoid overwriting
                var photoName = $"{Guid.NewGuid()}{fileExtension}";

                // Set upload folder within wwwroot for web access
                string uploadsFolder = Path.Combine("wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Absolute path for saving the photo
                string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), uploadsFolder, photoName);

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await Input.Photo.CopyToAsync(stream);
                }

                // Save relative path for easy retrieval in the application
                photoPath = $"/uploads/{photoName}";
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // First, create IdentityUser to generate password hash
                    var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (!result.Succeeded)
                    {
                        ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                        await transaction.RollbackAsync();
                        return Page();
                    }

                    // Now, create the User with the hashed password from IdentityUser
                    var newUser = new User
                    {
                        FullName = Input.FullName,
                        CreditCardNo = encryptedCreditCard,
                        Gender = Input.Gender,
                        MobileNo = Input.MobileNo,
                        DeliveryAddress = Input.DeliveryAddress,
                        Email = Input.Email,
                        PhotoPath = photoPath,
                        AboutMe = Input.AboutMe,
                        PasswordHash = user.PasswordHash, // Set PasswordHash before saving
                        EncryptionIV = creditCardIV // Store IV for later decryption
                    };

                    // Add User after setting PasswordHash
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                    
                    // Decrypt the credit card number for display
                    DecryptedCreditCardNo = _encryptionHelper.Decrypt(newUser.CreditCardNo, newUser.EncryptionIV);

                    return RedirectToPage("/Home");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ErrorMessage = $"An error occurred during registration: {ex.InnerException?.Message ?? ex.Message}";
                    return Page();
                }
            }
        }

        // JsonResult method for real-time email checking
        public JsonResult OnGetCheckEmail(string email)
        {
            bool isEmailTaken = _context.Users.Any(u => u.Email == email);
            return new JsonResult(new { isEmailTaken });
        }

        // Password Strength Checker (returns strength level 0-5)
        private int CheckPasswordStrength(string password)
        {
            int score = 0;

            if (password.Length >= 12)
                score++;
            if (password.Any(char.IsLower) && password.Any(char.IsUpper))
                score++;
            if (password.Any(char.IsDigit))
                score++;
            if (password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>/?".Contains(ch)))
                score++;
            if (password.Length >= 16)
                score++;  // Extra point for very long passwords

            return score;
        }
    }
}
