using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dit230680c_AS.Models
{
    public class PasswordHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("IdentityUser")]  // ✅ Ensure it's linked to IdentityUser
        public string UserId { get; set; }

        [Required]
        public string HashedPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ Default to UTC
    }
}
