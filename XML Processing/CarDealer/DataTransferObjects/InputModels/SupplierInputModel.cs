using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.InputModels
{
    //<Supplier>
    //<name>Airgas, Inc.</name>
    //<isImporter>false</isImporter>
    //</Supplier>

    [XmlType("Supplier")]
    public class SupplierInputModel
    {
       
        [XmlElement("name")]
        public  string Name { get; set; }
        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }

    }
}
