using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Entities
{
    public class UserEntity : IdentityUser
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string LastName { get; set; }
    }
}
