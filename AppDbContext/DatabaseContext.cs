using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using Entities;

namespace AppDbContext
{
    public class DatabaseContext : IdentityDbContext<UserEntity, IdentityRole, String>
    {
        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<MediaEntity> Media { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options) { }
    }
}
