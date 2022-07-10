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
    public class CreateRoomConsumer : IConsumer<CreateRoomModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<CreateRoomConsumer> _logger;

        public CreateRoomConsumer(ILogger<CreateRoomConsumer> logger, DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateRoomModel> context)
        {
            try
            {
                bool roomExists = (await _dbContext.Rooms.Where(x => x.Name == context.Message.Name).FirstOrDefaultAsync()) != null;

                if (roomExists)
                {
                    await context.RespondAsync(
                    new ResponseEntity
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = "Room with this name already exists"
                    });
                    return;
                }

                RoomEntity room = new()
                {
                    Name = context.Message.Name,
                    Description = context.Message.Description,
                    NoOfPeople = context.Message.NoOfPeople,
                    PriceForNight = Math.Round(context.Message.PriceForNight,2),
                    Count = context.Message.Count,
                    Photos = context.Message.Files
                };

                await _dbContext.AddAsync(room);

                var result = await _dbContext.SaveChangesAsync();

                await context.RespondAsync(
                    new ResponseEntity
                    {
                        Code = HttpStatusCode.OK,
                        Message = "Room created successfully!"
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
