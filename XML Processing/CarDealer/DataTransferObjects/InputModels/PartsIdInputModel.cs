using System.Xml.Serialization;

namespace CarDealer.DataTransferObjects.InputModels
{
    //  <partId id = "39" />

    [XmlType("partId")]
    public class PartsIdInputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

    }
}