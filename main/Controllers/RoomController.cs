using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Entities;
using hotel.Helpers;
using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Models;

namespace hotel.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    //[Authorize("Administrator,User")]
    public class RoomController : ControllerBase
    {
        readonly IRequestClient<CreateRoomModel> _createRoomClient;
        readonly IRequestClient<RoomListEntity> _getRoomClient;

        public RoomController(IRequestClient<CreateRoomModel> createRoomClient, IRequestClient<RoomListEntity> getRoomClient)
        {
            _createRoomClient = createRoomClient;
            _getRoomClient = getRoomClient;
        }

        [HttpPost]
        [Authorize("Administrator")]
        public async Task<IActionResult> CreateRoomAsync([FromBody] CreateRoomModel room)
        {
            var response = await _createRoomClient.GetResponse<ResponseEntity>(room);

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoom()
        {
            RoomListEntity request = new RoomListEntity() { rooms= new List<RoomEntity>() { }};

            var response = await _getRoomClient.GetResponse<RoomEntity[], ResponseEntity>(request);


            if (response.Is(out Response<RoomEntity[]> roomsFound))
            {
                return StatusCode((int)HttpStatusCode.OK, roomsFound.Message);
            }
            else if (response.Is(out Response<ResponseEntity> errorResponse))
            {
                return StatusCode((int)errorResponse.Message.Code, roomsFound.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "");
        }
    }
}
