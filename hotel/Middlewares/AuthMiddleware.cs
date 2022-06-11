using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using hotel.Entities;
using hotel.Helpers;
using hotel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace hotel.Middlewares
{
    public class AuthMiddleware
    {
        private ITokenHelper _tokenHelper;

        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next, ITokenHelper tokenHelper)
        {
            _next = next;
            _tokenHelper = tokenHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                attachAccountToContext(context, token);
            }
            await _next(context);
        }

        private void attachAccountToContext(HttpContext context, string token)
        {
            try
            {
                JwtUserEntity user = _tokenHelper.ValidateJwtToken(token);
                context.Items["UserId"] = user.UserId;
                context.Items["roles"] = user.roles;
            }
            catch
            {
            }
        }
    }
}
