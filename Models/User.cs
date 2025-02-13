using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace dit230680c_AS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }  // Full Name

        [Required]
        public string CreditCardNo { get; set; }  // Original Credit Card Number (will remain in plain text during registration)

        [NotMapped]  // We won't store the raw encrypted credit card number here directly
        public string EncryptedCreditCardNo { get; set; } // Store the encrypted credit card number (in your database)

        [NotMapped]  // We won't store the IV directly in the plain-text field but will use it for decryption later
        public string EncryptionIV { get; set; } // IV used during encryption (this will be stored in the database)

        [Required]
        public string Gender { get; set; }  // Gender

        [Required, StringLength(20)]
        public string MobileNo { get; set; }  // Mobile Number

        [Required, StringLength(255)]
        public string DeliveryAddress { get; set; }  // Delivery Address

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }  // Email address (must be unique)

        [Required]
        public string PasswordHash { get; set; }  // Store hashed password

        [NotMapped]
        [Required, Compare("PasswordHash", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }  // Used only for validation, not stored in DB

        [StringLength(255)]
        public string PhotoPath { get; set; }  // Photo (File path to the uploaded photo)

        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }  // About Me (allow all special characters)

        public string? CurrentSessionId { get; set; }  // Allow null value (for session management)
    }
}
