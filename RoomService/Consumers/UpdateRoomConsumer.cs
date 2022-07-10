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
    public class UpdateRoomConsumer : IConsumer<UpdateRoomModel>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<CreateRoomConsumer> _logger;
        private readonly GetRoomConsumer getRoomConsumer;


        public UpdateRoomConsumer(ILogger<CreateRoomConsumer> logger, DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateRoomModel> context)
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

                if(context.Message.Name != null)
                {
                    room.Name = context.Message.Name;
                }

                if (context.Message.Description != null)
                {
                    room.Description = context.Message.Description;
                }

                if (context.Message.NoOfPeople != null)
                {
                    room.NoOfPeople = (int)context.Message.NoOfPeople;
                }

                if (context.Message.PriceForNight != null)
                {
                    room.PriceForNight = Math.Round((decimal)context.Message.PriceForNight,2);
                }

                await _dbContext.SaveChangesAsync();

                await context.RespondAsync(
                    new ResponseEntity
                    {
                        Code = HttpStatusCode.OK,
                        Message = "Room updated successfully"
                    });
                return;

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
