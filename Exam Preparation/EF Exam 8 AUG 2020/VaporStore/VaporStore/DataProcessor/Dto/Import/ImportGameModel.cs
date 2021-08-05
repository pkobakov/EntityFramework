using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
    class ImportGameModel
        //the ImportModel should exactly reproduce the view of the json (string properties, DateTime and values)
    {
        [Required]
        public string  Name { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Genre { get; set; }
        public List<string> Tags { get; set; }
    }

 
}
