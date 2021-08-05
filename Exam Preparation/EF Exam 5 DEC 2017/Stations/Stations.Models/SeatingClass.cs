using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.Models
{
    //Id – integer, Primary Key
    //Name – text with max length 30 (required, unique)
    //Abbreviation – text with an exact length of 2 (no more, no less), (required, unique)

   public class SeatingClass
    {
        public SeatingClass()
        {
            TrainSeats = new HashSet<TrainSeat>();
        }
        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength(2)]
        public string Abbreviation { get; set; }

        public ICollection<TrainSeat> TrainSeats { get; set; }
    }
}
