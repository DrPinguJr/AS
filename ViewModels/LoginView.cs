using System.ComponentModel.DataAnnotations;

namespace dit230680c_AS.ViewModels
{
    public class LoginView
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
