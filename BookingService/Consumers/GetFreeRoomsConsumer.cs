using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Models;

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookingService.Consumers
{
    public class GetFreeRoomsConsumer : IConsumer<GetFreeRoomsModel>
    {
        private readonly ILogger<GetFreeRoomsConsumer> _logger;
        private readonly DatabaseContext _dbContext;

        public GetFreeRoomsConsumer(ILogger<GetFreeRoomsConsumer> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GetFreeRoomsModel> context)
        {
            List<RoomEntity> bookings = await (from booking in _dbContext.Bookings
                                               where booking.DateTo > context.Message.DateFrom && booking.DateFrom < context.Message.DateTo
                                               select booking.Room
                                                  ).ToListAsync();

            List<GetRoomModel> rooms = await (from room in _dbContext.Rooms
                                             where room.NoOfPeople >= context.Message.NumberOfPeople && !room.Deleted
                                             select room
                                               )
                                               .Distinct()
                                               .Select(x => new GetRoomModel()
                                               {
                                                   Id = x.Id,
                                                   Name = x.Name,
                                                   NoOfPeople = x.NoOfPeople,
                                                   PriceForNight = x.PriceForNight,
                                                   Description = x.Description,
                                                   Count = x.Count,
                                                   Photos = x.Photos.Select(x => x.Id).ToList()
                                               })
                                               .ToListAsync();

            var result = rooms.Where(p => bookings.Count(p2 => p2.Id == p.Id)<p.Count).ToList();

            var response = new FreeRoomListModel() { rooms = result };

            await context.RespondAsync(response);
        }
    }
}
