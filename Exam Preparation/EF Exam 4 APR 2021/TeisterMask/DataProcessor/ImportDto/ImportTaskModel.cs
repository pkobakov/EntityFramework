using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportTaskModel
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } // 2,40

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate  { get; set; }

        [Range(0,3)]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }

        [Range(0,4)]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }


    }
}
