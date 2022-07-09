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
using Models;

namespace RoomService.Consumers
{
    public class GetRoomConsumer : IConsumer<RoomListModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GetRoomConsumer> _logger;

        public GetRoomConsumer(ILogger<GetRoomConsumer> logger, DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoomListModel> context)
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

        public async Task<GetRoomModel[]> FindRoomAsync(List<GetRoomModel> rooms)
        {
            if (rooms == null || rooms.Count == 0)
            {
                var response = await _dbContext
                    .Rooms
                    .Where(x => !x.Deleted)
                    .Include(x=>x.Photos)
                    .Select(x=> new GetRoomModel() {
                        Id = x.Id,
                        Name = x.Name,
                        NoOfPeople = x.NoOfPeople,
                        PriceForNight = x.PriceForNight,
                        Description = x.Description,
                        Count = x.Count,
                        Photos = x.Photos.Select(x=>x.Id).ToList()
                    })
                    .ToArrayAsync();

                return response;
            }
            else
            {
                var roomsList = await
                    _dbContext
                    .Rooms
                    .Include(x=>x.Photos)
                    .Where(x=>!x.Deleted)
                    .Select(x => new GetRoomModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        NoOfPeople = x.NoOfPeople,
                        PriceForNight = x.PriceForNight,
                        Count = x.Count,
                        Photos = x.Photos.Select(x => x.Id).ToList()
                    })
                    .ToArrayAsync();

                var response = roomsList.Where(x => rooms.Exists(z => z.Id == x.Id)).ToArray();

                return response;
            }
        }
    }
}
