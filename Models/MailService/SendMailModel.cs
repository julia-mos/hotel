using System;
namespace Models
{
    public class SendMailModel
    {
        public string Receiver { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
