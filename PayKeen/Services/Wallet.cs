using PayKeen.Controllers.Api;
using PayKeen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Services
{
    
    // Keyword Retrieve
    //phone : Phone;
    //SMS: balance +pin
    public class Wallet
    {
        private readonly ApplicationDbContext _context;
        public Wallet()
        {
            _context = new ApplicationDbContext();
        }
        public string Balance (string phone, string[] tokens)
        {
            decimal balance = 0.0M;
            int pin = 0;
            int truePin = 0;
            string fullName = "";
            int tokenLength = tokens.Length;

            if (tokenLength > 2 || tokenLength < 2)
            {
                DefaultController.message = "invalid message format. Please enter `Balance + pin`";
                return DefaultController.message;
            }
            else
            {
              truePin = int.Parse(tokens[1]);
            }
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
                    pin = int.Parse(merchant.MerchantShortCode);
                    fullName = merchant.FirstName + " " + merchant.LastName;
                }
            }
            else
            {
                pin = int.Parse(customers.Pin);
                fullName = customers.FirstName + " " + customers.LastName;
            }

            if(truePin == pin)
            {
                var customerRequesting = _context.CWallet.Where(c => string.Equals(c.Phone, phone)).FirstOrDefault()?.Id ?? -1;

                if (customerRequesting == -1)
                {
                    var merchantRequesting = _context.MWallet.Where(m => string.Equals(m.Phone, phone)).FirstOrDefault()?.Id ?? -1;
                    if (merchantRequesting == -1)
                        DefaultController.message = "Sorry, your phone number is not registered with PayKeen";
                    else
                    {
                        MWallet wallet = _context.MWallet.Find(merchantRequesting);
                        balance = wallet.Balance;
                        DefaultController.message = "Sucess!, Hello " + fullName + ", your wallet balance is #" + balance;
                        return DefaultController.message;
                    }
                }
                else
                {
                    CWallet wallet = _context.CWallet.Find(customerRequesting);
                    balance = wallet.Balance;
                    DefaultController.message = "Success!, Hello " + fullName + ", your wallet balance is #" + balance;
                    return DefaultController.message;
                }
            }
            else
            {
                DefaultController.message = "invalid pin";
                return DefaultController.message;
            }
            
                
            return DefaultController.message;
        }
    }
}
