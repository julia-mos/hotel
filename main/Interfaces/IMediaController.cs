using System;
using System.Threading.Tasks;
using Entities;
using hotel.Entities;
using Microsoft.AspNetCore.Http;

namespace hotel.Interfaces
{
    public interface IMediaController
    {
        Task<MediaEntity> UploadFileAsync(IFormFile file);
    }
}
