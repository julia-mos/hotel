using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace main.Controllers
{
    [ApiController]
    [Route("/api/bookings")]

    public class BookingController : ControllerBase
    {
        readonly IRequestClient<GetFreeRoomsModel> _freeRoomsClient;
        readonly ILogger<BookingController> _logger;

        public BookingController(IRequestClient<GetFreeRoomsModel> freeRoomsClient, ILogger<BookingController> logger)
        {
            _freeRoomsClient = freeRoomsClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("free-rooms")]
        public async Task<IActionResult> GetFreeRooms(string dateFrom, string dateTo, int people)
        {
            try
            {
                DateTime dateFromParsed = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime dateToParsed = DateTime.ParseExact(dateTo, "yyyyMMdd", CultureInfo.InvariantCulture);

                var request = new GetFreeRoomsModel() { DateFrom = dateFromParsed, DateTo = dateToParsed, NumberOfPeople = people };

                var response = await _freeRoomsClient.GetResponse<ResponseEntity>(request);

                return StatusCode((int)response.Message.Code, response.Message.Message);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while reading data");
            }

        }

    }
}
