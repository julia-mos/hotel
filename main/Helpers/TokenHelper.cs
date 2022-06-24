using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using hotel.Entities;
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
        private readonly List<string> roles;
        public AuthorizeAttribute(string Roles) : base()
        {
            roles = Roles.Split(",").ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var UserId = context.HttpContext.Items["UserId"];
            List<string> userRoles = context.HttpContext.Items["roles"] as List<string>;

            if (roles.Count > 0 && userRoles != null && userRoles.Count > 0)
            {
                bool hasRole = userRoles.Any(x => roles.Any(y => y == x));

                if(!hasRole)
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (UserId == null || (roles.Count > 0 && userRoles.Count == 0))
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


        public JwtUserEntity? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            string secret = Environment.GetEnvironmentVariable("JWT_SECRET");

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
                List<string> roles = jwtToken.Claims.First(x => x.Type == "roles").Value.Split(",").ToList();

                return new JwtUserEntity(UserId, roles);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }
    }
}
