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
    public class GetBookingConsumer : IConsumer<GetBookingsModel>
    {
        private readonly ILogger<GetBookingConsumer> _logger;
        private readonly DatabaseContext _dbContext;

        public GetBookingConsumer(ILogger<GetBookingConsumer> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GetBookingsModel> context)
        {
            try
            {
                if (context.Message.userId != null)
                {
                    var userBookings = await _dbContext.Bookings.Where(x => x.User.Id == context.Message.userId).Include(x => x.Room).Include(x=>x.User).ToArrayAsync();

                    await context.RespondAsync(userBookings);
                }
                else
                {
                    var allBookings = await _dbContext.Bookings.Include(x=>x.Room).Include(x => x.User).ToArrayAsync();

                    await context.RespondAsync(allBookings);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
            }
        }
    }
}
