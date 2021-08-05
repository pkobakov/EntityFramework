using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerModel
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3)] [MaxLength(30)]
        public string Fullname { get; set; }

        [XmlElement("Money")]
        [Required]
        [Range(typeof(decimal),"0", "79228162514264337593543950335")]
        public decimal Money { get; set; }

        [XmlElement("Position")]
        [Required]
        public string Position { get; set; }

        [XmlElement("Weapon")]
        [Required]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public ImportOfficerPrisonerModel[] Prisoners {get;set;}

    }
}
