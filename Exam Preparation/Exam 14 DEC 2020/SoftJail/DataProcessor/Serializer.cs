namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners.Where(p => ids.Contains(p.Id)).ToList()
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(o=> new 
                    { 
                    
                    OfficerName = o.Officer.FullName,
                    Department = o.Officer.Department.Name,
                   
                    }).OrderBy(o=>o.OfficerName).ToList(),

                   TotalOfficerSalary = p.PrisonerOfficers.Sum(p=>p.Officer.Salary)

                }).OrderBy(p=>p.Name).ThenBy(p=>p.Id).ToList();


            var jsonexport = JsonConvert.SerializeObject(prisoners, Formatting.Indented);
            return jsonexport;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonerlist = prisonersNames.Split(',').ToList();

            var prisoners = context.Prisoners
                                   .Where(p => prisonerlist.Contains(p.FullName))
                                   .ToList()
                                   .Select(p => new ExportPrisoners
                                   {
                                       Id = p.Id,
                                       Name = p.FullName,
                                       IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                       EncryptedMessages = p.Mails.ToArray().Select(m => new ExportMessagesDto
                                       {
                                           Description = ReverseString(m.Description)


                                       })
                                   .ToArray()
                                   })
                                   .OrderBy(p => p.Name)
                                   .ThenBy(p => p.Id)
                                   .ToList();


            string root = "Prisoners";
            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportPrisoners>), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, String.Empty);
            serializer.Serialize(textwriter, prisoners, ns);

            var result = textwriter.ToString().Trim();

            return result;

        }

        static string ReverseString(string text) 
        {
            var charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        
        }
    }
}