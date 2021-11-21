using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class Customers
    {
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Pin { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string BVN { get; set; }

        public string Status { get; set; }
        public DateTime DateEnrolled { get; set; }
    }
}
