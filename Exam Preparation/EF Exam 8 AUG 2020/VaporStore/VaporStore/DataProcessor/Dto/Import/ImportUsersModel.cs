using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{

    public class ImportUsersModel
    {
       

        [Required]
        [RegularExpression(@"(?<FirstName>[A-Z]{1}[a-z]{2,}) (?<LastName>[A-Z]{1}[a-z]{2,})")]
        public string FullName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3,103)]
        public int Age { get; set; }
        public List<CardInputModel> Cards { get; set; }
    }

    public class CardInputModel
    {
        [Required]
        [RegularExpression(@"[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{3}")]
        public string CVC { get; set; }

        [Required]
        public CardType? Type { get; set; }
    }

    
}
