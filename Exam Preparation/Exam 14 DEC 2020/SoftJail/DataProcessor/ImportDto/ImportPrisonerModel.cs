using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
   //FullName": "",
   //Nickname": "The Wallaby",
   //Age": 32,
   //IncarcerationDate": "29/03/1957",
   //ReleaseDate": "27/03/2006",
   //Bail": null,
   //CellId": 5,
   //Mails": [
   // {
   //   "Description": "Invalid FullName",
   //   "Sender": "Invalid Sender",
   //   "Address": "No Address"
   // },

    public class ImportPrisonerModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^The\s[A-Z]{1}[a-z]+$")]
        public string Nickname { get; set; }
        [Required]
        [Range(18,65)]
        public int Age { get; set; }
        [Required]
        public string IncarcerationDate { get; set; }
        public string? ReleaseDate { get; set; }

       
        [Range(typeof(decimal), "0","79228162514264337593543950335")]
        public decimal? Bail { get; set; }
        public int? CellId { get; set; }
        public ImportMailModel[] Mails { get; set; }
    }
}
