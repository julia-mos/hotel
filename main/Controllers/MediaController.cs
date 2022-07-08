using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using hotel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace main.Controllers
{
    public class MediaController : IMediaController
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<MediaController> _logger;

        public MediaController(ILogger<MediaController> logger, DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<MediaEntity> UploadFileAsync(IFormFile file) {
            try
            {
                var folderName = Path.Combine("Uploads");

                FileInfo fi = new FileInfo(file.FileName);
                string ext = fi.Extension;

                var fileName = Guid.NewGuid().ToString();

                var dbPath = Path.Combine(folderName, $"{fileName}{ext}");

                using (var stream = new FileStream(dbPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var dbFileModel = new MediaEntity()
                {
                    Path = dbPath,
                    Id  = fileName
                };

                var dbFile = await _dbContext.Media.AddAsync(dbFileModel);

                await _dbContext.SaveChangesAsync();

                return dbFile.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return null;
            }

        }
    }
}
