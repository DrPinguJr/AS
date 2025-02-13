using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;  // For IFormFile

namespace dit230680c_AS.ViewModels
{
    public class RegisterView
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }  // Full Name

        [Required]
        public string CreditCardNo { get; set; }  // Must be encrypted

        [Required]
        public string Gender { get; set; }  // Gender

        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; }  // Mobile Number

        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; }  // Delivery Address

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }  // Email address (must be unique)

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Password

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }  // Confirm Password

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }  // Date of Birth

        [StringLength(500)]
        public string AboutMe { get; set; }  // About Me (allow all special characters)

        // Photo Upload (.JPG only)
        [Required]
        public IFormFile Photo { get; set; }  // Photo (.JPG only)
    }
}
