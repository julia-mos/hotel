using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Database;
using AuthService.Entities;
using AuthService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, AppDbContext dbContext, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<UserEntity>> Get()
        {
            return await _dbContext.Users.ToArrayAsync();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            bool userExists = await _userManager.FindByNameAsync(model.Email) != null;

            if (userExists)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseEntity { Status = StatusEnum.Error.ToString(), Message = "User with this email already exists" });

            UserEntity user = new () 
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseEntity { Status = StatusEnum.Error.ToString(), Message = "Cannot register. Check details and try again." });

            return Ok(new ResponseEntity { Status = StatusEnum.Success.ToString(), Message = "User created successfully!" });
        }
    }
}
