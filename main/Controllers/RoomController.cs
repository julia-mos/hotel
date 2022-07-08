using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Entities;
using hotel.Helpers;
using hotel.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace hotel.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomController : ControllerBase
    {
        readonly IRequestClient<CreateRoomModel> _createRoomClient;
        readonly IRequestClient<RoomListEntity> _getRoomClient;
        readonly IRequestClient<DeleteRoomModel> _deleteRoomClient;
        readonly IRequestClient<UpdateRoomModel> _updateRoomClient;
        readonly IMediaController _mediaController;


        public RoomController(
            IRequestClient<CreateRoomModel> createRoomClient,
            IRequestClient<RoomListEntity> getRoomClient,
            IRequestClient<DeleteRoomModel> deleteRoomClient,
            IRequestClient<UpdateRoomModel> updateRoomClient,
            IMediaController mediaController
        )
        {
            _createRoomClient = createRoomClient;
            _getRoomClient = getRoomClient;
            _deleteRoomClient = deleteRoomClient;
            _updateRoomClient = updateRoomClient;
            _mediaController = mediaController;
        }

        [HttpPost]
        [Authorize("Administrator")]
        public async Task<IActionResult> CreateRoom([FromForm] CreateRoomModel room, IFormFileCollection files)
        {
            List<MediaEntity> uploadedFiles = new List<MediaEntity>();

            if(files.Count > 0)
            {
                foreach (var file in files)
                {
                    uploadedFiles.Add(await _mediaController.UploadFileAsync(file));
                }
            }

            var response = await _createRoomClient.GetResponse<ResponseEntity>(room);

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> GetRoom(int? id = null)
        {
            RoomListEntity request = new RoomListEntity() { rooms= new List<RoomEntity>() { }};

            if(id != null)
            {
                request.rooms.Add(new RoomEntity(){ Id = (int)id });
            }


            var response = await _getRoomClient.GetResponse<RoomEntity[], ResponseEntity>(request);


            if (response.Is(out Response<RoomEntity[]> roomsFound))
            {
                return StatusCode((int)HttpStatusCode.OK, roomsFound.Message);
            }
            else if (response.Is(out Response<ResponseEntity> errorResponse))
            {
                return StatusCode((int)errorResponse.Message.Code, errorResponse.Message.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "");
        }

        [HttpPut]
        [Authorize("Administrator")]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomModel roomModel)
        {
            roomModel.Id = id;

            var response = await _updateRoomClient.GetResponse<ResponseEntity>(roomModel);

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }

        [HttpDelete]
        [Authorize("Administrator")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var response = await _deleteRoomClient.GetResponse<ResponseEntity>(new DeleteRoomModel() { Id = id });

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }
    }
}
