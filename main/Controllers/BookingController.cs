using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Entities;
using hotel.Helpers;
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
        readonly IRequestClient<MakeBookingModel> _makeBookingClient;
        readonly ILogger<BookingController> _logger;

        public BookingController(
            IRequestClient<GetFreeRoomsModel> freeRoomsClient,
            ILogger<BookingController> logger,
            IRequestClient<MakeBookingModel> makeBookingClient
            )
        {
            _freeRoomsClient = freeRoomsClient;
            _logger = logger;
            _makeBookingClient = makeBookingClient;
        }

        [HttpGet]
        [Route("free-rooms")]
        public async Task<IActionResult> GetFreeRooms(string dateFrom, string dateTo, int people)
        {
            try
            {
                DateTime dateFromParsed = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime dateToParsed = DateTime.ParseExact(dateTo, "yyyyMMdd", CultureInfo.InvariantCulture);

                dateFromParsed = dateFromParsed.AddHours(15);
                dateToParsed = dateToParsed.AddHours(11);

                var request = new GetFreeRoomsModel() { DateFrom = dateFromParsed, DateTo = dateToParsed, NumberOfPeople = people };

                var response = await _freeRoomsClient.GetResponse<FreeRoomListModel>(request);

                return StatusCode(200, response.Message);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while reading data");
            }

        }


        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> MakeBooking(string dateFrom, string dateTo, int people, int roomId)
        {
            try
            {
                DateTime dateFromParsed = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime dateToParsed = DateTime.ParseExact(dateTo, "yyyyMMdd", CultureInfo.InvariantCulture);

                dateFromParsed = dateFromParsed.AddHours(15);
                dateToParsed = dateToParsed.AddHours(11);

                var request = new MakeBookingModel() {
                    DateFrom = dateFromParsed,
                    DateTo = dateToParsed,
                    NumberOfPeople = people,
                    RoomId = roomId,
                    UserId = (string)HttpContext.Items["UserId"]
                };

                var response = await _makeBookingClient.GetResponse<ResponseEntity>(request);

                return StatusCode((int) response.Message.Code, response.Message.Message);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while reading data");
            }

        }

    }
}
