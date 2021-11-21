using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class CWallet
    {
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; }
        public string FullName { get; set; }

        public decimal Balance { get; set; }
    }
}
