using System;
namespace hotel.Interfaces
{
    public interface ITokenHelper
    {
        string? ValidateJwtToken(string token);
    }
}
