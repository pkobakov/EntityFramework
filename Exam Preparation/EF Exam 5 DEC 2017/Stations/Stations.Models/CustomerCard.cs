using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.Models
{
    public class CustomerCard
    {
        public CustomerCard()
        {
            BoughtTickets = new HashSet<Ticket>();
        }
        [Key]
        public int Key { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public int Age { get; set; }

        [Required]
        public CardType Type { get; set; }

        public ICollection<Ticket> BoughtTickets { get; set; }
    }
}
