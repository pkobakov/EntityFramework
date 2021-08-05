using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Stations.Models
{

    //Id – integer, Primary Key
    //TrainId – integer(required)
    //Train – train whose seats will be described(required)
    //SeatingClassId – integer(required)
    //SeatingClass – class of the seats(required)
    //Quantity – how many seats of given class total for the given train(required, non-negative)
    
   public class TrainSeat
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Train))]
        public int TrainId { get; set; }
        public Train Train { get; set; }

        [ForeignKey(nameof(SeatingClass))]
        public int SeatingClassId { get; set; }
        public SeatingClass SeatingClass { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
