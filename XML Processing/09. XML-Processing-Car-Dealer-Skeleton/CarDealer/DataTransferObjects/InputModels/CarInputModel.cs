using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.InputModels
{
    //<Car>
    //<make>Opel</make>
    //<model>Astra</model>
    //<TraveledDistance>516628215</TraveledDistance>
    //<parts>
    //  <partId id = "39" />
    //  < partId id="62"/>
    //  <partId id = "72" />
    //</ parts >
    // Car >
    [XmlType("Car")]
    public class CarInputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model  { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public PartsIdInputModel[] PartsIds { get; set; }

    }
}
