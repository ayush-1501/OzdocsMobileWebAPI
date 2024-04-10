using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Text;

namespace OzdocsMobileWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpPost("SendMail")]
        public IActionResult SendEmail(string feedback) // Change parameter type to string
        {
            try
            {
                if (string.IsNullOrWhiteSpace(feedback))
                {
                    return BadRequest("Feedback cannot be empty.");
                }

                string fromAddress = "ayushdel15@gmail.com";
                string toAddress = "ayushstarc@gmail.com";
                string fromPassword = "xmro mldw wfgg qhne";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(new MailAddress(toAddress));
                message.Subject = "Feedback";
                message.Body = feedback;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, fromPassword)
                };
                smtpClient.Send(message);

                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "An error occurred while sending the email: " + ex.Message);
            }
        }
    }
}
