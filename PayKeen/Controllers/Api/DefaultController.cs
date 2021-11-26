using Microsoft.AspNetCore.Mvc;
using PayKeen.Services;

namespace PayKeen.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        public static string message { get; set; } = "error occured";

        readonly Recharge recharge = new();
        readonly Payment pay = new();
        readonly CreateUser create = new();
        readonly Wallet wallet = new();
        readonly PinReset reset = new();


        [HttpPost]
        public string Main([FromForm] string from, [FromForm] string sms)
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
