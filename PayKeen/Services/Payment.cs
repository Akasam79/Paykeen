using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using PayKeen.Controllers.Api;
using PayKeen.Models;
using PayKeen.Services;

namespace PayKeen.Services
{
    public class Payment
    {
        private ApplicationDbContext _context;

        public Payment()
        {
            _context = new ApplicationDbContext();
        }

        //keyword : PAY
        //phone : Phone;
        //SMS: pay mShortCode amount
        public string Payments(string phone, string[] data)
        {
            try
            {
                int tokenLength = data.Length;
                if (tokenLength > 3 || tokenLength < 3) return "Enter the Proper keywords. i.e Pay `Merchant short code` amount (No double spaces)";
                string merchantShortCode = "";
                string senderFName = "";
                string senderLName = "";

                string receiverFName = "";
                string receiverLName = "";
                string receiverPhone = "";
                string transID = "";

                string staticNum = "11";

                Random rand = new Random();
                string pin = staticNum + rand.Next(100, 1000000000).ToString();

                if (pin.Length > 7)
                    transID = pin.Substring(0, 7);

                if (tokenLength == 3)
                {
                    merchantShortCode = data[1];
                    decimal amount = decimal.Parse(data[2]);

                    var senderId = _context.Customers.Where(c => string.Equals(c.Phone, phone)).FirstOrDefault().Id;

                    if (senderId.Equals(null))
                    {
                        DefaultController.message = "Invalid user";
                    }

                    Customers sender = _context.Customers.Find(senderId);

                    senderFName = sender.FirstName;
                    senderLName = sender.LastName;
                    string senderFullName = senderFName + " " + senderLName;

                    var receiverId = _context.Merchants.Where(m => string.Equals(m.MerchantShortCode, merchantShortCode)).FirstOrDefault().Id;

                    if (receiverId.Equals(null))
                    {
                        DefaultController.message = "Incorrect/invalid merchant code";
                    }

                    Merchant receiver = _context.Merchants.Find(receiverId);

                    receiverFName = receiver.FirstName;
                    receiverLName = receiver.LastName;
                    receiverPhone = receiver.Phone;
                    string receiverFullName = receiverFName + " " + receiverLName;

                    var walletId = _context.CWallet.Where(w => string.Equals(w.Phone, phone)).FirstOrDefault().Id;

                    CWallet wallet = _context.CWallet.Find(walletId);

                    var walletBalance = wallet.Balance;


                    if (walletBalance < amount || amount <= 0)
                    {
                        DefaultController.message = "Insufficient Balance Transaction declined!";
                    }
                    else
                    {
                        ActivePayments active = new ActivePayments
                        {
                            Phone = phone,
                            TransactionId = transID,
                            Sender = senderFullName,
                            recipientPhone = receiverPhone,
                            Recipient = receiverFullName,
                            Amount = amount
                        };
                        _context.ActivePayments.Add(active);
                        DefaultController.message = "processing... Are you sure you want to pay " + receiverFullName + " the sum of #" + amount + "?"
                                   + " Reply with `confirm +your pin followed by " + transID + "` or 0 to cancel";
                        _context.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("internal server error " + ex);

            }
            return DefaultController.message;
        }

        // keyword: confirm
        // confirm PIN transID
        public string ConfirmPayment(string phone, string[] data)
        {
            int tokenLength = data.Length;
            if (tokenLength > 3 || tokenLength < 3)
            {
                DefaultController.message = "Pls enter the proper keyword as described prior to this message";
            }

                string pin = data[1];
            string inputId = data[2];
            string transId = "";
            try
            {
                var transactionId = _context.ActivePayments.Where(a => a.Phone == phone).FirstOrDefault().Id;
                ActivePayments paymentDetails = _context.ActivePayments.Find(transactionId);
                transId = paymentDetails.TransactionId;

                var senderId = _context.Customers.Where(c => string.Equals(c.Phone, phone)).FirstOrDefault().Id;
                if (inputId.Equals(transId))
                {
                    Customers senderDetails = _context.Customers.Find(senderId);
                    string truePin = senderDetails.Pin;

                    if (truePin.Equals(pin))
                    {
                        Payments payments = new Payments
                        {
                            Phone = paymentDetails.Phone,
                            MerchantName = paymentDetails.Recipient,
                            Amount = paymentDetails.Amount,
                            MadeBy = paymentDetails.Sender
                        };
                        var cWalletId = _context.CWallet.Where(c => c.Phone == phone).FirstOrDefault().Id;
                        var cWalletDetails = _context.CWallet.Find(cWalletId);


                        var mWalletId = _context.MWallet.Where(c => c.Phone == paymentDetails.recipientPhone).FirstOrDefault().Id;
                        var mWalletDetails = _context.MWallet.Find(mWalletId);

                        cWalletDetails.Balance -= paymentDetails.Amount;
                        mWalletDetails.Balance += paymentDetails.Amount;

                        _context.Payments.Add(payments);
                        _context.ActivePayments.Remove(paymentDetails);

                        var result = _context.SaveChanges();
                        Console.WriteLine(result);
                        if (result >= 4)
                        {
                            DefaultController.message = "Payment successful";

                        }
                        else
                        {
                            DefaultController.message = "update failed";
                        }

                    }
                    else
                    {
                        DefaultController.message = "Invalid Pin";
                    }
                }
                else
                {
                    DefaultController.message = "invalid transaction ID";
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("internal server error " + ex);

            }
            return DefaultController.message;
        }
    }
}
