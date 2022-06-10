using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using hotel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace hotel.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var UserId = context.HttpContext.Items["UserId"];
            if (UserId == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }

    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            string secret = _configuration.GetSection(nameof(AppSecrets))
                   .Get<AppSecrets>()
                   .JWT.Secret;

            Console.WriteLine(secret);


            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string UserId = jwtToken.Claims.First(x => x.Type == "id").Value;
                Console.WriteLine(UserId);

                return UserId;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }
    }
}
