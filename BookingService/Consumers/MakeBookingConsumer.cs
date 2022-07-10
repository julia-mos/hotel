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
using System.Net;

namespace BookingService.Consumers
{
    public class MakeBookingConsumer : IConsumer<MakeBookingModel>
    {
        private readonly ILogger<MakeBookingConsumer> _logger;
        private readonly DatabaseContext _dbContext;
        private readonly IRequestClient<SendMailModel> _mailSender;

        public MakeBookingConsumer(ILogger<MakeBookingConsumer> logger, DatabaseContext dbContext, IRequestClient<SendMailModel> mailSender)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mailSender = mailSender;
        }

        public async Task Consume(ConsumeContext<MakeBookingModel> context)
        {
            try
            {
                var bookings = await (
                    from booking in _dbContext.Bookings
                    where booking.DateTo > context.Message.DateFrom && booking.DateFrom < context.Message.DateTo && booking.Room.Id == context.Message.RoomId
                    select booking
                ).ToListAsync();

                var selectedRoom = await (from room in _dbContext.Rooms
                                          where room.NoOfPeople >= context.Message.NumberOfPeople && room.Id == context.Message.RoomId && !room.Deleted
                                          select room
                                                   )
                                                   .Distinct()
                                                   .FirstOrDefaultAsync();

                if(selectedRoom == null)
                {
                    await context.RespondAsync(
                        new ResponseEntity() { Code = HttpStatusCode.NotFound, Message = "Room not found" }
                        );
                    return;
                }

                if (bookings.Count >= selectedRoom.Count)
                {
                    await context.RespondAsync(
                        new ResponseEntity() { Code = HttpStatusCode.NotFound, Message = "This room isn't available in those dates" }
                        );
                    return;
                }


                var user = await _dbContext.Users.Where(x => x.Id == context.Message.UserId).FirstOrDefaultAsync();

                int nights = (int)Math.Floor((context.Message.DateTo - context.Message.DateFrom).TotalDays);


                var dbBooking = new BookingEntity()
                {
                    DateFrom = context.Message.DateFrom,
                    DateTo = context.Message.DateTo,
                    NoOfPeople = context.Message.NumberOfPeople,
                    Price = Math.Round(nights * selectedRoom.PriceForNight,2),
                    Room = selectedRoom,
                    User = user
                };

                await _dbContext.Bookings.AddAsync(dbBooking);

                await _dbContext.SaveChangesAsync();

                var mail = new SendMailModel()
                {
                    Receiver = user.Email,
                    Subject = "HOTEL - thank you for your booking",
                    Body = $"" +
                    $"You just booked stay at {selectedRoom.Name} room at dates {context.Message.DateFrom.ToString("dd.MM.yyyy")}-{context.Message.DateTo.ToString("dd.MM.yyyy")}. <br/>" +
                    $"Total price to pay at hotel: {Math.Round(nights * selectedRoom.PriceForNight, 2)}zł"
                };


                await _mailSender.GetResponse<ResponseEntity>(mail);


                await context.RespondAsync(
                    new ResponseEntity()
                    {
                        Code = HttpStatusCode.OK,
                        Message = "Booking created"
                    }
                    );
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
            }
        }
    }
}
