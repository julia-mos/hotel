using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace MediaService.Controllers
{
    public class AddMediaConsumer : IConsumer<AddMediaModel>
    {
        public Task Consume(ConsumeContext<AddMediaModel> context)
        {
            throw new NotImplementedException();
        }
    }
}
