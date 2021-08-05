using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class AuthorBookInputModel
    {
        [JsonProperty("Id")]
        public int? BookId { get; set; }
       
    }
}
