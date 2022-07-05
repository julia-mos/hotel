using System;
using System.Net;
using System.Threading.Tasks;
using AppDbContext;
using Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Models;

namespace RoomService.Consumers
{
    public class CreateRoomConsumer : IConsumer<CreateRoomModel>
    {
        private readonly DatabaseContext _dbContext;

        public CreateRoomConsumer(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
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
                    PriceForNight = context.Message.PriceForNight
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
