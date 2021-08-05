using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    
    public class ImportPurchaseModel
    {
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Type")]
        public PurchaseType? Type { get; set; }

        [XmlElement("Key")]
        [RegularExpression(@"[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}")]
        public string Key { get; set; }

        [XmlElement("Card")]
        [RegularExpression(@"[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}")]
        public string Card { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }

    }
}
