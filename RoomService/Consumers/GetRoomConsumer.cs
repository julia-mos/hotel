using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace RoomService.Consumers
{
    public class GetRoomConsumer : IConsumer<RoomListEntity>
    {
        private readonly DatabaseContext _dbContext;

        public GetRoomConsumer(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<RoomListEntity> context)
        {
            try
            {
                if (context.Message.rooms == null || context.Message.rooms.Count == 0)
                {
                    var rooms = await _dbContext.Rooms.ToArrayAsync();

                    await context.RespondAsync(rooms);
                }
                else
                {
                    var rooms = await _dbContext.Rooms.ToListAsync();

                    var response = rooms.Where(x => context.Message.rooms.Exists(z => z.Id == x.Id)).ToArray();

                    await context.RespondAsync(response);
                }

            }
            catch
            {
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
