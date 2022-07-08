using System;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
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
                RoomEntity room = new()
                {
                    Name = context.Message.Name,
                    Description = context.Message.Description,
                    NoOfPeople = context.Message.NoOfPeople,
                    PriceForNight = context.Message.PriceForNight,
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
