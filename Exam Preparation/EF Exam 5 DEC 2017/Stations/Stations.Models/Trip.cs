using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Stations.Models
{
    //Id – integer, Primary Key
    //OriginStationId – integer(required)
    //OriginStation – station from which the trip begins(required)
    //DestinationStation Id – integer(required)
    //DestinationStation –  station where the trip ends(required)
    //DepartureTime – date and time of departure from origin station(required)
    //ArrivalTime – date and time of arrival at destination station, must be after departure time(required)
    //TrainId – integer(required)
    //Train – train used for that particular trip(required)
    //Status – TripStatus enumeration with possible values: "OnTime", "Delayed" and "Early" (default: "OnTime")!!!!
    //TimeDifference – time(span) representing how late or early a given train was(optional)

    public class Trip
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(OriginStation))]
        public int OriginStationId { get; set; }
        public Station OriginStation { get; set; }

        [ForeignKey(nameof(DestinationStation))]
        public int DestinationStationId { get; set; }
        public Station DestinationStation { get; set; }

        [Required]
        public DateTime? ArrivalTime { get; set; }

        [ForeignKey(nameof(Train))]
        public int TrainId { get; set; }
        public Train Train { get; set; }

        [Required]
        public TripStatus? Status { get; set; }

        public TimeSpan? TimeDifference { get; set; }

    }
}
