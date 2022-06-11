using System;
using System.Threading.Tasks;
using hotel.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace hotel.Controllers
{
    [ApiController]
    [Route("/")]
    [Authorize("Admin,User")]
    public class TestController : ControllerBase
    {

        public TestController()
        {

        }

        [HttpGet]
        public string Get()
        {
            return "test";
        }
    }
}
