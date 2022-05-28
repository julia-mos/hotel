using System;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(3)]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(3)]
        [MaxLength(100)]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
