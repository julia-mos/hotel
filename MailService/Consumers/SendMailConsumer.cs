using System;
using System.Net;
using System.Threading.Tasks;
using Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeKit;
using Models;

namespace MailService
{
    public class SendMailConsumer : IConsumer<SendMailModel>
    {

        private readonly ILogger<SendMailConsumer> _logger;

        public SendMailConsumer(ILogger<SendMailConsumer> logger)
        {
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<SendMailModel> context)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(Environment.GetEnvironmentVariable("SMTP_SENDER"));
                email.To.Add(MailboxAddress.Parse(context.Message.Receiver));
                email.Subject = context.Message.Subject;

                var builder = new BodyBuilder();

                builder.HtmlBody = context.Message.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(
                    Environment.GetEnvironmentVariable("SMTP_HOST"),
                    int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")),
                    SecureSocketOptions.StartTls
                );
                smtp.Authenticate(Environment.GetEnvironmentVariable("SMTP_USERNAME"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));

                await smtp.SendAsync(email);

                smtp.Disconnect(true);

                await context.RespondAsync(new ResponseEntity() { Code = HttpStatusCode.OK, Message = "Mail sent" });
            }

            catch(Exception exc)
            {
                _logger.LogError(exc.Message);

                await context.RespondAsync(new ResponseEntity() { Code = HttpStatusCode.InternalServerError, Message = "Couldn't send email" });
            }
        }
    }
}
