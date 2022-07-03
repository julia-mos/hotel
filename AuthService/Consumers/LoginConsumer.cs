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
    public class LoginConsumer : IConsumer<LoginModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;

        public LoginConsumer(DatabaseContext dbContext, UserManager<UserEntity> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<LoginModel> context)
        {
            UserEntity user = await _userManager.FindByNameAsync(context.Message.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, context.Message.Password))
            {
                IList<string> userRoles = await _userManager.GetRolesAsync(user);

                JwtSecurityToken token = GenerateToken(user.Id, String.Join(",", userRoles.ToArray()));

                await context.RespondAsync(
                    new TokenEntity { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo }
                );
            }
            else
            await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                });
        }

        private JwtSecurityToken GenerateToken(string userID, string userRoles)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));


            var token = new JwtSecurityToken(
                claims: new[] { new Claim("id", userID), new Claim("roles", userRoles) },
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(secret, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
