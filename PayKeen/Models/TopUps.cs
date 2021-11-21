using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class TopUps
    {
        public int Id { get; set; }
        public string MadeBy { get; set; }

        [Required]
        public string AdminPhone { get; set; }
        public string RecipientPhone { get; set; }

        public string RecipientName { get; set; }

        public string Amount { get; set; }

        public DateTime MadeAt { get; set; }
    }
}
