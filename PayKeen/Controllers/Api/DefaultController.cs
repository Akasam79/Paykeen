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

        Recharge recharge = new Recharge();
        Payment pay = new Payment();
        CreateUser create = new CreateUser();
        Wallet wallet = new Wallet();
        PinReset reset = new PinReset();


        public DefaultController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpPost]
        public string main([FromForm] string from, [FromForm] string sms)
        {
            string[] parameters = sms.Split(null);
            string keyword = parameters[0].ToLower();

            switch (keyword)
            {
                case "enroll":
                    create.AddUser(from, parameters);
                    break;

                case "pay":
                    pay.Payments(from, parameters);
                    break;

                case "confirm":
                    pay.ConfirmPayment(from, parameters);
                    break;

                case "recharge":
                    recharge.TopUp(from, parameters);
                    break;

                case "confirmr":
                    recharge.ConfirmTopUp(from, parameters);
                    break;

                case "balance":
                    wallet.Balance(from, parameters);
                    break;

                case "reset":
                    reset.ResetPin(from, parameters);
                    break;

                default:
                    DefaultController.message = "Invalid keyword";
                    return DefaultController.message;
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
