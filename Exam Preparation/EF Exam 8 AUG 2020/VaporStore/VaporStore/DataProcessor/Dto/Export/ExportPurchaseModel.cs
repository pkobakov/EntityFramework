using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using VaporStore.ExportResults;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Purchase")]
    public class ExportPurchaseModel
    {
        [XmlElement("Card")]
        public string CardNumber{ get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        public ExportGameXml Game{ get; set; }
    }
}
