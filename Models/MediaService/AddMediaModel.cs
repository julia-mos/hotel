using System;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class AddMediaModel
    {
        public IFormFile File { get; set; }
    }
}
