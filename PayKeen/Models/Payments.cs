using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class Payments
    {
        public int Id { get; set; }

        [Required]
        public string Phone { get; set; }

        public string MerchantName { get; set; }

        public decimal Amount { get; set; }

        public string MadeBy { get; set; }

        public DateTime MadeAt { get; set; }
    }
}
