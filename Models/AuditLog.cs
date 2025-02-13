using System;
using System.ComponentModel.DataAnnotations;

namespace dit230680c_AS.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // The ID of the user performing the action

        [Required]
        public string Activity { get; set; }  // The description of the activity

        [Required]
        public DateTime Timestamp { get; set; }  // When the activity happened

        public string IpAddress { get; set; }  // The IP address of the user (optional)

        public string UserAgent { get; set; }  // The browser information (optional)
    }
}
