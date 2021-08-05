using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Message")]
    public class ExportMessagesDto
    {
        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }
}