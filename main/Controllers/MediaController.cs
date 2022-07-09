using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using hotel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace main.Controllers
{
    [ApiController]
    [Route("/api/uploads")]
    public class MediaController : ControllerBase, IMediaController
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

                var dbPath = Path.GetFullPath(Path.Combine(folderName, $"{fileName}{ext}"));

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

                return dbFileModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return null;
            }

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetPhotoAsync(string id)
        {
            var file = await _dbContext.Media.Where(x => x.Id == id).FirstOrDefaultAsync();

            return PhysicalFile(file.Path, "image/jpeg");
        }
    }
}
