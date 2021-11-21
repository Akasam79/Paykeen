using Microsoft.Data.SqlClient;
using PayKeen.Controllers.Api;
using PayKeen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Services
{
    public class CreateUser
    {
        private ApplicationDbContext _context;
        public CreateUser()
        {
            _context = new ApplicationDbContext();
        }
        public string AddUser(string from, string[] data)
        {

            try
            {
                int tokenLength = data.Length;
                Console.WriteLine(tokenLength);
                string userType = data[1].ToString();

                if (tokenLength > 5 || tokenLength < 4) return "pls enter valid keywords as directed";
                if (tokenLength == 4 && userType.Equals("merchant"))
                {
                    return "merchant must have firstName, LastName and BVN in that order";
                }
                else if (tokenLength == 5 && userType.Equals("merchant"))
                {

                    String status = "Inactive";
                    string phone = from;
                    string bvn = data[4];
                    string firstName = data[2];
                    string lastName = data[3];
                    string fullName = firstName + " " + lastName;
                    int staticCode = 22;
                    decimal defaultBalance = 0;
                    Random rand = new Random();
                    string merchantShortCode = staticCode + rand.Next(100, 10000).ToString();

                    var user = new Users
                    {
                        Phone = phone,
                        BVN = bvn,
                        UserType = userType,
                        FirstName = firstName,
                        LastName = lastName,
                        Status = status,
                        DateEnrolled = DateTime.Now
                    };
                    var merchant = new Merchant
                    {
                        Phone = phone,
                        MerchantShortCode = merchantShortCode,
                        FirstName = firstName,
                        LastName = lastName,
                        BVN = bvn,
                        Status = status,
                        DateEnrolled = DateTime.Now
                    };

                    var mwallet = new MWallet
                    {
                        Phone = phone,
                        FullName = fullName,
                        Balance = defaultBalance
                    };

                    _context.Users.Add(user);
                    _context.Merchants.Add(merchant);
                    _context.MWallet.Add(mwallet);
                    try
                    {
                        int result = _context.SaveChanges();
                        if( result == 3)
                        {
                            DefaultController.message = "Successfully added user";

                        }
                        else
                        {
                            DefaultController.message = "error occured when creating account";
                        }

                    }
                    catch (SqlException e)
                    {
                        if (e.Number == 2601)
                        {
                            DefaultController.message = "Phone number is already registered with PayKeen";
                        }
                        else
                        {
                            DefaultController.message = "internal server error " + e;
                        }
                    }
                }
                else if (tokenLength == 4 || tokenLength == 5 && userType.Equals("customer"))
                {
                    string status = "Inactive";
                    string phone = from;
                    string bvn = data[4];
                    string firstName = data[2];
                    string lastName = data[3];
                    string fullName = firstName + " " + lastName;
                    int staticCode = 100;
                    int defaultBalance = 0;
                    Random rand = new Random();
                    string pin = staticCode + rand.Next(100, 10000000).ToString();

                    if (pin.Length > 6)
                    {
                        string newPin = pin.Substring(0, 6);


                        var user = new Users
                        {
                            Phone = phone,
                            BVN = bvn,
                            UserType = userType,
                            FirstName = firstName,
                            LastName = lastName,
                            Status = status,
                            DateEnrolled = DateTime.Now
                        };
                        var customer = new Customers
                        {
                            Phone = phone,
                            Pin = newPin,
                            FirstName = firstName,
                            LastName = lastName,
                            BVN = bvn,
                            Status = status,
                            DateEnrolled = DateTime.Now
                        };

                        var cwallet = new CWallet
                        {
                            Phone = phone,
                            FullName = fullName,
                            Balance = defaultBalance
                        };

                        _context.Users.Add(user);
                        _context.Customers.Add(customer);
                        _context.CWallet.Add(cwallet);
                        DefaultController.message = "Successfully added user";
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
    }
}
