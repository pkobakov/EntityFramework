using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stations.Models
{
    //Id – integer, Primary Key
    //TrainNumber – text with max length 10 (required, unique)
    //Type – TrainType enumeration with possible values: "HighSpeed", "LongDistance" or "Freight" (optional)
    //TrainSeats – Collection of type TrainSeat
    //Trips – Collection of type Trip

    public class Train
    {
        public Train()
        {
            TrainSeats = new HashSet<TrainSeat>();
            Trips = new HashSet<Trip>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string  TrainNumber { get; set; }

        public TrainType Type { get; set; }
        public ICollection<TrainSeat> TrainSeats { get; set; }
        public ICollection<Trip> Trips { get; set; }

    }
}
