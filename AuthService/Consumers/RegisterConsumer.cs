using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AuthService.Consumers
{
    public class RegisterConsumer : IConsumer<RegisterModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IRequestClient<SendMailModel> _mailSender;

        public RegisterConsumer(DatabaseContext dbContext, UserManager<UserEntity> userManager, IRequestClient<SendMailModel> mailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mailSender = mailSender;
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
                        Code = HttpStatusCode.InternalServerError,
                        Message = "Couldn't create user"
                    });

            string confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;


            var mail = new SendMailModel() {
                Receiver = context.Message.Email,
                Subject = "HOTEL - confirm your email",
                Body = $"<a href='http://localhost:8000/api/users/verify?userId={user.Id}&token={Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken))}'>Click on the link to verify your account</a>" };


            await _mailSender.GetResponse<ResponseEntity>(mail);

           
            await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.OK,
                    Message = "User created successfully!"
                });
        }
    }
}
