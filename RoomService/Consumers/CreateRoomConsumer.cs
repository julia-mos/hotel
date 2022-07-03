using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace RoomService.Consumers
{
    public class CreateRoomConsumer : IConsumer<CreateRoomModel>
    {
        public Task Consume(ConsumeContext<CreateRoomModel> context)
        {
            throw new NotImplementedException();
        }
    }
}
