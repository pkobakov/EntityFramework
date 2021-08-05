
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.Models
{
    //Id – integer, Primary Key
    //Name – text with max length 50 (required, unique)
    //Town – text with max length 50 (required)
    //TripsTo – Collection of type Trip
    //TripsFrom – Collection of type Trip

    public class Station
    {
        public Station()
        {
            TripsTo = new HashSet<Trip>();
            TripsFrom = new HashSet<Trip>();
        }
        [Key]
        public int Id { get; set; }
     
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Town { get; set; }

        public ICollection<Trip> TripsTo { get; set; }
        public ICollection<Trip> TripsFrom { get; set; }
    }
}
