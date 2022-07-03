using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthService.Database;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AuthService.Consumers
{
    public class RegisterConsumer : IConsumer<RegisterModel>
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;

        public RegisterConsumer(AppDbContext dbContext, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<RegisterModel> context)
        {
            bool userExists = await _userManager.FindByNameAsync(context.Message.Email) != null;

            if (userExists)
                await context.RespondAsync(
                    new ResponseEntity
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = "User with this email already exists"
                    }); ;

            UserEntity user = new()
            {
                Email = context.Message.Email,
                UserName = context.Message.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName
            };

            var result = await _userManager.CreateAsync(user, context.Message.Password);

            await _userManager.AddToRoleAsync(user, "User");

            if (!result.Succeeded)
                await context.RespondAsync(
                    new ResponseEntity
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = "User with this email already exists"
                    });

            await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.OK,
                    Message = "User created successfully!"
                });
        }
    }
}
