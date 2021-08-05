namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentsCells = JsonConvert.DeserializeObject<ImportDepartmentModel[]>(jsonString);
            var output = new StringBuilder();
            List<Department> departments = new List<Department>();

            foreach (var departmentCellModel in departmentsCells)
            {
                if (!IsValid(departmentCellModel))
                {
                    output.AppendLine(ErrorMessage);continue;
                }

                var department = new Department 
                {
                    Name  = departmentCellModel.Name,
                   
                };

                List<Cell> cells = new List<Cell>();

                foreach (var cellModel in departmentCellModel.Cells)
                {
                    if (!IsValid(cellModel))
                    {
                        output.AppendLine(ErrorMessage);break;
                    }
                    var cell = new Cell 
                    { 
                    CellNumber  =cellModel.CellNumber,
                    Department = department,
                    HasWindow = cellModel.HasWindow
                    };

                    department.Cells.Add(cell);
                  

                    
                }

                if (department.Cells.Count!=0)
                {
                    departments.Add(department);
                    output.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
                }
               
                
                
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();
            return output.ToString().Trim();
             
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonersMailsModels = JsonConvert.DeserializeObject<ImportPrisonerModel[]>(jsonString);
            var output = new StringBuilder();
            var prisoners = new List<Prisoner>();

            foreach (var currentModel in prisonersMailsModels)
            {
                if (!IsValid(currentModel))
                {
                    output.AppendLine(ErrorMessage); continue;
                }


                DateTime incdate;
                var incarcerationDateValodation = DateTime.TryParseExact(currentModel.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out incdate);
                DateTime releasedate;
                var releaseDateValdation = DateTime.TryParseExact(currentModel.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out releasedate);
                var prisoner = new Prisoner
                {
                    FullName = currentModel.FullName,
                    Nickname = currentModel.Nickname,
                    Age = currentModel.Age,
                    IncarcerationDate = incdate,
                    ReleaseDate = releasedate,
                    Bail = currentModel.Bail,
                    CellId = currentModel.CellId

                };
                    var  mails = new List<Mail>();

                foreach (var currentmail in currentModel.Mails)
                {
                    var mail = new Mail() 
                    { 
                    
                    Description = currentmail.Description,
                    Sender = currentmail.Sender,
                    Address = currentmail.Address,
                    Prisoner = prisoner
                    
                    };

                    if (!IsValid(mail.Address))
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    mails.Add(mail);
                }

                prisoner.Mails = mails;
                prisoners.Add(prisoner);
                output.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return output.ToString().Trim();
            
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            string root = "Officers";
            XmlSerializer serializer = new XmlSerializer(typeof(ImportOfficerModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(xmlString);
            var importedOfficerModel = serializer.Deserialize(textRead) as ImportOfficerModel[];

            var officers = new List<Officer>();
            var output = new StringBuilder();

            foreach (var currentOfficer in importedOfficerModel)
            {
                if (!IsValid(currentOfficer))
                {
                    output.AppendLine(ErrorMessage);continue;
                }

                Position position;
                var positiontype = Enum.TryParse(currentOfficer.Position, out position);

                if (!positiontype)
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                Weapon weapon;
                bool weapontype = Enum.TryParse(currentOfficer.Weapon, out weapon);

                if (!weapontype)
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                var officer = new Officer 
                { 
                FullName = currentOfficer.Fullname,
                Salary = currentOfficer.Money,
                Position = position, 
                Weapon  = weapon,
                DepartmentId = currentOfficer.DepartmentId
                };

                foreach (var currentPrisoner in currentOfficer.Prisoners)
                {
                    var prisonerId = new OfficerPrisoner 
                    { 
                        Officer = officer,
                      PrisonerId = currentPrisoner.Id
                    };

                    officer.OfficerPrisoners.Add(prisonerId);
                }

                officers.Add(officer);
                output.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();
            return output.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}