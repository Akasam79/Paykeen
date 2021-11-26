using PayKeen.Controllers.Api;
using PayKeen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Services
{
    public class Recharge
    {
        private ApplicationDbContext _context;

        
        public int adminPin = 0;

        public Recharge()
        {
            _context = new ApplicationDbContext();
        }
        // Recharge
        // SMS : recharge phone + amount
        // returns : are you sure you want to top-up "fullName" with "amount"? reply with PIN + transID
        public string TopUp(string phone, string[] tokens)
        {
            string firstName = "";
            string lastName = "";
            string adminName = "";
            string transId = "";
            string customerPhone = "";
            string customerName = "";
            int amount = 0;

            int tokenLength = tokens.Length;
            string staticNum = "11";

            Random rand = new Random();
            string pin = staticNum + rand.Next(100, 1000000000).ToString();
            if (pin.Length > 7)
                transId = pin.Substring(0, 7);
            if (tokenLength == 3)
            {
                customerPhone = tokens[1];
                amount = int.Parse(tokens[2]);
            }
            else
                DefaultController.message = "invalid/improper keyword";
            try
            {
                var depositor = _context.outletAdmins.Where(d => string.Equals(d.Phone, phone)).FirstOrDefault().Id;
                OutletAdmins outletAdmins = _context.outletAdmins.Find(depositor);
                Console.WriteLine(outletAdmins.FullName);
                adminName = outletAdmins.FullName;
                adminPin = outletAdmins.Pin;
                try
                {
                    var receiver = _context.Customers.Where(r => string.Equals(r.Phone, customerPhone)).FirstOrDefault()?.Id ?? -1;
                    Customers customers = _context.Customers.Find(receiver);
                    Console.WriteLine(receiver);
                    if (receiver == -1)
                    {
                        var mReceiver = _context.Merchants.Where(m => string.Equals(m.Phone, customerPhone)).FirstOrDefault()?.Id ?? -1;
                        Merchant merchant = _context.Merchants.Find(mReceiver);
                        Console.WriteLine(mReceiver);
                        if (mReceiver == -1)
                        {
                            DefaultController.message = "Hello " + adminName + " Please inform your customer that their number is not registered with PayKeen yet";
                            return DefaultController.message;
                        }
                            
                        else
                        {
                            firstName = merchant.FirstName;
                            lastName = merchant.LastName;
                            customerName = firstName + " " + lastName;
                        }
                    }
                    else
                    {
                        firstName = customers.FirstName;
                        lastName = customers.LastName;
                        customerName = firstName + " " + lastName;
                    }
                }
                catch(NullReferenceException ex)
                {
                    DefaultController.message = ex.Message;
                }
                



                ActivePayments active = new ActivePayments
                {
                    Phone = phone,
                    TransactionId = transId,
                    Sender = adminName,
                    recipientPhone = customerPhone,
                    Recipient = customerName,
                    Amount = amount
                };

                _context.ActivePayments.Add(active);
                DefaultController.message = "processing... Are you sure you want to top up " + customerName + "'s account with the sum of #" + amount + "?"
                           + " Reply with `confirmR +your pin followed by " + transId + "` or 0 to cancel";
                _context.SaveChanges();
            }
            catch(NullReferenceException)
            {
                DefaultController.message = "Transaction Declined! Invalid Agent";
            }
            

            return DefaultController.message;
        }

        //Keyword: ConfirmR
        // sms: ConfirmRecharge + pin + transID
        public string ConfirmTopUp(string phone, string[] tokens)
        {
            int tokenLength = tokens.Length;
            adminPin = int.Parse(tokens[1]);
            if (tokenLength > 3 || tokenLength < 3)
            {
                DefaultController.message = "Pls enter the proper keyword as described prior to this message";
            }

            int pin = int.Parse(tokens[1]);
            string inputId = tokens[2];
            string transId = "";
            try
            {
                var transactionId = _context.ActivePayments.Where(a => a.Phone == phone).FirstOrDefault();
                transId = transactionId.TransactionId;

                var senderId = _context.outletAdmins.Where(c => string.Equals(c.Phone, phone)).FirstOrDefault().Id;
                if (inputId.Equals(transId))
                {
                    OutletAdmins senderDetails = _context.outletAdmins.Find(senderId);
                    int truePin = senderDetails.Pin;
                    Console.WriteLine(truePin);

                    if (truePin == pin)
                    {
                        TopUps topUps = new TopUps
                        {
                            MadeBy = transactionId.Sender,
                            AdminPhone = phone,
                            RecipientPhone = transactionId.recipientPhone,
                            RecipientName = transactionId.Recipient,
                            Amount = transactionId.Amount

                        };
                        var cWalletId = _context.CWallet.Where(c => c.Phone == transactionId.recipientPhone).FirstOrDefault()?.Id ?? -1;
                        if (cWalletId== -1)
                        {
                            var mWalletId = _context.MWallet.Where(c => c.Phone == transactionId.recipientPhone).FirstOrDefault()?.Id ?? -1;
                            var mWalletDetails = _context.MWallet.Find(mWalletId);
                            mWalletDetails.Balance += transactionId.Amount;

                        }
                        else
                        {
                            var cWalletDetails = _context.CWallet.Find(cWalletId);
                            cWalletDetails.Balance += transactionId.Amount;
                        }
                        

                        _context.TopUps.Add(topUps);
                        _context.ActivePayments.Remove(transactionId);

                        var result = _context.SaveChanges();
                        Console.WriteLine(result);
                        if (result >= 3)
                        {
                            DefaultController.message = "Payment successful";
                            return DefaultController.message;

                        }
                        else
                        {
                            DefaultController.message = "update failed";
                            return DefaultController.message;
                        }

                    }
                    else
                    {
                        DefaultController.message = "Invalid Pin";
                        return DefaultController.message;
                    }
                }
                else
                {
                    DefaultController.message = "invalid transaction ID";
                    return DefaultController.message;
                }
            }
            catch (Exception ex)
            {

                DefaultController.message = "internal server error" + ex.Message;
            }
            return DefaultController.message;
        }
    }
    
}
