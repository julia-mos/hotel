using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace RoomService.Consumers
{
    public class DeleteRoomConsumer : IConsumer<DeleteRoomModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<DeleteRoomConsumer> _logger;

        public DeleteRoomConsumer(ILogger<DeleteRoomConsumer> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<DeleteRoomModel> context)
        {
            try
            {
                var room = await _dbContext.Rooms.Where(x => x.Id == context.Message.Id && !x.Deleted).FirstOrDefaultAsync();

                if(room == null)
                {
                    await context.RespondAsync(
                        new ResponseEntity
                        {
                            Code = HttpStatusCode.NotFound,
                            Message = "Room not found"
                        });

                    return;
                }

                room.Deleted = true;

                await _dbContext.SaveChangesAsync();

                await context.RespondAsync(
                        new ResponseEntity
                        {
                            Code = HttpStatusCode.OK,
                            Message = "Room deleted successfully"
                        });
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                await context.RespondAsync(
                new ResponseEntity
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Something went wrong"
                });
            }
        }
    }
}
