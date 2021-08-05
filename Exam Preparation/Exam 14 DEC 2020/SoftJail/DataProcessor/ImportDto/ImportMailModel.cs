using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
   public class ImportMailModel
    {
        public string  Description { get; set; }
        public string Sender  { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s]+\sstr.$")]
        public string Address { get; set; }

    }
}
