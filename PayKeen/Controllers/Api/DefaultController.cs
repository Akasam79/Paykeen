using Microsoft.AspNetCore.Mvc;
using PayKeen.Models;
using PayKeen.Services;
using System;
using System.Net.Http.Formatting;

namespace PayKeen.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private ApplicationDbContext _context;
        public static string message { get; set; } = "error occured";

        public DefaultController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpPost]
        public string main([FromForm] string from, [FromForm] string sms)
        {
            string[] parameters = sms.Split(null);

            if (string.Equals(parameters[0], "enroll", StringComparison.OrdinalIgnoreCase))
            {
                CreateUser create = new CreateUser();
                create.AddUser(from, parameters);
            }
            else if (string.Equals(parameters[0], "pay", StringComparison.OrdinalIgnoreCase))
            {
                Payment pay = new Payment();
                pay.payment(from, parameters);

            }
            else if (string.Equals(parameters[0], "confirm", StringComparison.OrdinalIgnoreCase))
            {
                Payment pay = new Payment();
                pay.confirmPayment(from, parameters);
            }
            return message;
        }
        [HttpGet]
        public string GetDefaultRoute()
        {
            return "SetUp complete";
        }
    }
}
