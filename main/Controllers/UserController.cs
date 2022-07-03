﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using hotel.Helpers;
using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Models;

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

        public UserController(IRequestClient<UserListEntity> userClient, IRequestClient<RegisterModel> registerClient, IRequestClient<LoginModel> loginClient)
        {
            _userClient = userClient;
            _registerClient = registerClient;
            _loginClient = loginClient;
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
        [Route("")]
        [Authorize("Administrator")]
        public async Task<IActionResult> GetUsers()
        {
            UserListEntity request = new UserListEntity() { users= new List<UserEntity>() { }};

            var response = await _userClient.GetResponse<UserEntity[]>(request);

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

            var response = await _userClient.GetResponse<UserEntity[]>(request);

            return StatusCode(200, response.Message);
        }
    }
}