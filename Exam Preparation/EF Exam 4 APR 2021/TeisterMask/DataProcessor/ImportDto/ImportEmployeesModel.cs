using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
   public class ImportEmployeesModel
    {
        [JsonProperty("Username")]
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Username { get; set; }

        [JsonProperty("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonProperty("Phone")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
        public string Phone { get; set; }
        public List<int> Tasks { get; set; }

    }
}
