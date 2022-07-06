using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RoomService.Consumers
{
    public class GetRoomConsumer : IConsumer<RoomListEntity>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GetRoomConsumer> _logger;

        public GetRoomConsumer(ILogger<GetRoomConsumer> logger, DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoomListEntity> context)
        {
            try
            {
                if (context.Message.rooms == null || context.Message.rooms.Count == 0)
                {
                    var rooms = await _dbContext.Rooms.Where(x=>!x.Deleted).ToArrayAsync();

                    await context.RespondAsync(rooms);

                    return;
                }
                else
                {
                    var rooms = await _dbContext.Rooms.ToListAsync();

                    var response = rooms.Where(x => context.Message.rooms.Exists(z => z.Id == x.Id) && !x.Deleted).ToArray();

                    if(response.Count() == 0)
                    {
                        await context.RespondAsync(
                        new ResponseEntity
                        {
                            Code = HttpStatusCode.NotFound,
                            Message = "Room not found"
                        });

                        return;
                    }

                    await context.RespondAsync(response);
                }

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
