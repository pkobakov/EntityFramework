using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.OutputModels
{
    [XmlType("car")]
    public class CarPartOutputModel
    {
        [XmlAttribute("make")]
        public string Make  { get; set; }
        [XmlAttribute("model")]
        public string Model  { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
        
        [XmlArray("parts")] 
        public PartsInfo[] Parts { get; set; }

    }
    [XmlType("part")]
    public class PartsInfo
    {
        [XmlAttribute("name")]
        public string PartName  { get; set; }
        [XmlAttribute("price")]
        public decimal Price { get; set; }

    }
}
