using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace AuthService.Consumers
{
    public class VerifyEmailConsumer : IConsumer<VerifyEmailModel>
    {
        private readonly UserManager<UserEntity> _userManager;

        public VerifyEmailConsumer(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<VerifyEmailModel> context)
        {
            UserEntity user = await _userManager.FindByIdAsync(context.Message.Id);

            if(user == null)
            {
                await context.RespondAsync(
                                new ResponseEntity
                                {
                                    Code = HttpStatusCode.NotFound,
                                    Message = "User not found"
                                });
                return;
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(Convert.FromBase64String(context.Message.token)));

            if (result.Succeeded)
            {
                await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.OK,
                    Message = "You can now log in"
                });
            }
            else
            {
                await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Error while confirming email"
                });
            }
        }

    }
}
