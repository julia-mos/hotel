using System;
using System.Collections.Generic;
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
        readonly IRequestClient<BookingListModel> _getBookingClient;

        readonly ILogger<BookingController> _logger;

        public BookingController(
            IRequestClient<GetFreeRoomsModel> freeRoomsClient,
            ILogger<BookingController> logger,
            IRequestClient<MakeBookingModel> makeBookingClient,
            IRequestClient<BookingListModel> getBookingClient
            )
        {
            _freeRoomsClient = freeRoomsClient;
            _logger = logger;
            _makeBookingClient = makeBookingClient;
            _getBookingClient = getBookingClient;
        }

        [HttpGet]
        [Route("free-rooms")]
        public async Task<IActionResult> GetFreeRooms(string dateFrom, string dateTo, int people)
        {
            try
            {
                DateTime dateFromParsed = DateTime.ParseExact(dateFrom, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime dateToParsed = DateTime.ParseExact(dateTo, "yyyyMMdd", CultureInfo.InvariantCulture);

                if (dateFromParsed >= dateToParsed)
                    return StatusCode(400, "Date from can't be later than date to");

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

                if(dateFromParsed >= dateToParsed)
                    return StatusCode(400, "Date from can't be later than date to");

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


        [HttpGet]
        [Authorize()]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                BookingListModel request = new BookingListModel() { bookings = new List<BookingEntity>() { } };

                List<string> userRoles = HttpContext.Items["roles"] as List<string>;

                if (!userRoles.Contains("Administrator"))
                {
                    request.userId = (string)HttpContext.Items["UserId"];
                }


                var response = await _getBookingClient.GetResponse<BookingEntity[]>(request);

                return StatusCode(200, response.Message);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while reading data");
            }

        }
    }
}
