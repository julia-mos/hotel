using AuthService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace AuthService.Database
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole, String>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
