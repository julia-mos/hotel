using System;
using System.Collections.Generic;
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
                var response = await FindRoomAsync(context.Message.rooms);

                if(response.Count() == 0 && context.Message.rooms.Count() > 0)
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

        public async Task<RoomEntity[]> FindRoomAsync(List<RoomEntity> rooms)
        {
            if (rooms == null || rooms.Count == 0)
            {
                var response = await _dbContext.Rooms.Where(x => !x.Deleted).ToArrayAsync();

                return response;
            }
            else
            {
                var roomsList = await _dbContext.Rooms.ToListAsync();

                var response = roomsList.Where(x => rooms.Exists(z => z.Id == x.Id) && !x.Deleted).ToArray();

                return response;
            }
        }
    }
}
