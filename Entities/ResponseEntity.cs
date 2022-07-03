using System.Net;

namespace Entities
{
    public class ResponseEntity
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }
}
