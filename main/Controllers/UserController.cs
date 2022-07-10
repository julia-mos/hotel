using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Entities;
using hotel.Helpers;
using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Models;
using Models.AuthService;

namespace hotel.Controllers
{
    [ApiController]
    [Route("/api/users")]
    //[Authorize("Administrator,User")]
    public class UserController : ControllerBase
    {
        readonly IRequestClient<UserListEntity> _userClient;
        readonly IRequestClient<RegisterModel> _registerClient;
        readonly IRequestClient<LoginModel> _loginClient;
        readonly IRequestClient<DeleteUserModel> _deleteClient;
        readonly IRequestClient<VerifyEmailModel> _verifyEmailClient;

        public UserController(
            IRequestClient<UserListEntity> userClient,
            IRequestClient<RegisterModel> registerClient,
            IRequestClient<LoginModel> loginClient,
            IRequestClient<DeleteUserModel> deleteClient,
            IRequestClient<VerifyEmailModel> verifyEmailClient
        )
        {
            _userClient = userClient;
            _registerClient = registerClient;
            _loginClient = loginClient;
            _deleteClient = deleteClient;
            _verifyEmailClient = verifyEmailClient;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel user)
        {
            var response = await _registerClient.GetResponse<ResponseEntity>(user);

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel user)
        {
            var response = await _loginClient.GetResponse<ResponseEntity, TokenEntity>(user);

            if (response.Is(out Response<ResponseEntity> responseEntity))
            {
                return StatusCode((int)responseEntity.Message.Code, responseEntity.Message.Message);
            }
            else if (response.Is(out Response<TokenEntity> tokenEntityResponse))
            {
                return StatusCode(200, tokenEntityResponse.Message);
            }

            return StatusCode(500);
        }

        [HttpGet]
        [Route("{id}?")]
        [Authorize("Administrator")]
        public async Task<IActionResult> GetUsers(string? id)
        {
            UserListEntity request = new UserListEntity() { users= new List<UserEntity>() { }};


            if(id != null)
            {
                request.users.Add(new UserEntity() { Id = id });
            }

            var response = await _userClient.GetResponse<GetUserModel[]>(request);

            return StatusCode(200, response.Message);
        }

        [HttpGet]
        [Route("me")]
        [Authorize()]
        public async Task<IActionResult> GetMe()
        {

            UserListEntity request = new UserListEntity() {
                users = new List<UserEntity>() {
                    new UserEntity() { Id = (string)HttpContext.Items["UserId"] }
                }
            };

            var response = await _userClient.GetResponse<GetUserModel[]>(request);

            return StatusCode(200, response.Message);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize()]
        public async Task<IActionResult> DeleteUser(string id)
        {
            List<string> userRoles = HttpContext.Items["roles"] as List<string>;

            if ((string)HttpContext.Items["UserId"] != id || !userRoles.Contains("Administrator"))
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Unauthorized");
            }

            var response = await _deleteClient.GetResponse<ResponseEntity>(new DeleteUserModel() { Id = id});

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }

        [HttpGet]
        [Route("verify")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var response = await _verifyEmailClient.GetResponse<ResponseEntity>(new VerifyEmailModel() { Id = userId, token=token });

            return StatusCode((int)response.Message.Code, response.Message.Message);
        }
    }
}
