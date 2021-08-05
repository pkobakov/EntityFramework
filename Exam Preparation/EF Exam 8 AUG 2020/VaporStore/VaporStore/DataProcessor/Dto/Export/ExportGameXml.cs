using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.ExportResults
{

    [XmlType("Game")]
   public class ExportGameXml
    {
        [XmlAttribute("title")]
        public string Name { get; set; }
      
        public decimal Price { get; set; }

       
        public string Genre { get; set; }

    }
}
