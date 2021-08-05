using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class ExportUserXml
    {
        [XmlAttribute("username")]
        public string UserName { get; set; }
        
      
        [XmlArray]
        public ExportPurchaseModel[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
