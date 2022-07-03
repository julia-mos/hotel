using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using Entities;

namespace AuthService.Database
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole, String>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
