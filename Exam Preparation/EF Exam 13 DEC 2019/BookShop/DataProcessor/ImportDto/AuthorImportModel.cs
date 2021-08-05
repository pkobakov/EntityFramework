using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    //"FirstName": "K",
    //"LastName": "Tribbeck",
    //"Phone": "808-944-5051",
    //"Email": "btribbeck0@last.fm",
    //"Books": [
    //  {
    //    "Id": 79
    //  },
    //  {
    //"Id": 40
    //  }
    public class AuthorImportModel
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3), MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]{3})\-([0-9]{3})\-([0-9]{4})$")]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public AuthorBookInputModel[] Books { get; set; }

    }
}
