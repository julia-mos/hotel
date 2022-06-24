using System;
using hotel.Entities;

namespace hotel.Interfaces
{
    public interface ITokenHelper
    {
        JwtUserEntity? ValidateJwtToken(string token);
    }
}
