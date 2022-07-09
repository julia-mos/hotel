using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace BookingService.Consumers
{
    public class GetFreeRoomsConsumer : IConsumer<GetFreeRoomsModel>
    {
        public Task Consume(ConsumeContext<GetFreeRoomsModel> context)
        {
            throw new NotImplementedException();
        }
    }
}
