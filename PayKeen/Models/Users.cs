using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; }
        public string UserType { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string BVN { get; set; }

        public string Status { get; set; }
        public DateTime DateEnrolled { get; set; }
    }
}
