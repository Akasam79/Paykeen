using PayKeen.Controllers.Api;
using PayKeen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Services
{
    //keyword: reset
    //phone : Phone;
    //SMS: reset DefaultPin NewPin
    public class PinReset
    {
        private readonly ApplicationDbContext _context;
        public PinReset()
        {
            _context = new ApplicationDbContext();
        }
        public string ResetPin (string phone, string[] tokens)
        {
            string fullName;
            int tokenLength = tokens.Length;
            int DefaultPin = 0;
            int newPin = 0;
            int enteredPin = 0;
            if (tokenLength < 3 || tokenLength > 3)
            {
                DefaultController.message = "incorrect text format";
                return DefaultController.message;
            }

            enteredPin = int.Parse(tokens[1]);
            newPin = int.Parse(tokens[2]);

            var customerId = _context.Customers.Where(r => string.Equals(r.Phone, phone)).FirstOrDefault()?.Id ?? -1;
            Customers customers = _context.Customers.Find(customerId);
            if (customerId == -1)
            {
                var merchantId = _context.Merchants.Where(m => string.Equals(m.Phone, phone)).FirstOrDefault()?.Id ?? -1;
                Merchant merchant = _context.Merchants.Find(merchantId);
                if (merchantId == -1)
                {
                    DefaultController.message = "Hello, your number is not registered with PayKeen yet";
                    return DefaultController.message;
                }

                else
                {
                    DefaultPin = int.Parse(merchant.MerchantShortCode);
                    fullName = merchant.FirstName + " " + merchant.LastName;
                    if (DefaultPin == enteredPin)
                    {
                        merchant.MerchantShortCode = newPin.ToString();
                    }
                    else
                    {
                        DefaultController.message = "incorrect DefaultPin, please enter your default pin and try again";
                        return DefaultController.message;
                    }
                }
            }
            else
            {
                DefaultPin = int.Parse(customers.Pin);
                fullName = customers.FirstName + " " + customers.LastName;
                if (DefaultPin == enteredPin)
                {
                    customers.Pin = newPin.ToString();
                }
                else
                {
                    DefaultController.message = "incorrect DefaultPin, please enter your default pin and try again";
                    return DefaultController.message;
                }
            }
            var result = _context.SaveChanges();
            if(result == 1)
            {
                DefaultController.message = "Success!, Hello " + fullName + " your new pin is " + newPin;
                return DefaultController.message;
            }
            

            return DefaultController.message;
        }
    }
}
