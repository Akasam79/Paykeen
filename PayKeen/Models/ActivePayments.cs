using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class ActivePayments
    {
        public int Id { get; set; }

        [Required]
        public string Phone { get; set; }
        public string TransactionId { get; set; }

        public string Sender { get; set; }
        public string recipientPhone { get; set; }

        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public DateTime InitiationTime { get; set; }
    }
}
