namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var output = new StringBuilder();
            string root = "Projects";
            var serializer = new XmlSerializer(typeof(ImportProjectModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(xmlString);
            var projectsModels = serializer.Deserialize(textRead) as ImportProjectModel[];

            List<Project> projects = new List<Project>();


            foreach (var currentProject  in projectsModels)
            {


                if (!IsValid(currentProject))
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                DateTime openDate;

                var isValidOpenDate = DateTime.TryParseExact(currentProject.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out openDate);

                if (!IsValid(isValidOpenDate))
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                DateTime? dueDate = null;

                if (!String.IsNullOrEmpty(currentProject.DueDate))
                {
                    DateTime currentProjectDueDate;

                    var isValidDueDate = DateTime.TryParseExact(currentProject.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out currentProjectDueDate);

                    if (!isValidDueDate)
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    dueDate = currentProjectDueDate;
                }

                else
                {
                    dueDate = null;
                }

                var project = new Project 
                { 
                Name = currentProject.Name,
                OpenDate = openDate,
                DueDate = dueDate
                };

                foreach (var currentTaskModel in currentProject.Tasks)
                {
                    if (!IsValid(currentTaskModel))
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    DateTime taskOpenDate;

                    var isValidTaskOpenDate = DateTime.TryParseExact(currentTaskModel.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate);

                    if (!isValidTaskOpenDate)
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    if (taskOpenDate < project.OpenDate)
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    DateTime taskDueDate;

                    var isValidDueDate = DateTime.TryParseExact(currentTaskModel.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out taskDueDate);

                    if (!isValidDueDate)
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    if (project.DueDate.HasValue)
                    {
                        if (taskDueDate>project.DueDate)
                        {
                            output.AppendLine(ErrorMessage);
                        }
                    }

                    var task = new Task
                    {
                        Name = currentTaskModel.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)currentTaskModel.ExecutionType,
                        LabelType = (LabelType)currentTaskModel.LabelType

                    };

                    project.Tasks.Add(task);
                }
                projects.Add(project);
                output.AppendLine(String.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return output.ToString().Trim();
        }








        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var importedEmployees = JsonConvert.DeserializeObject<List<ImportEmployeesModel>>(jsonString);

            var output = new StringBuilder();
            var employees = new List<Employee>();

            foreach (var currentEmployee in importedEmployees)
            {

                if (!IsValid(currentEmployee))
                {
                    output.AppendLine(ErrorMessage);continue;
                }


                var employee = new Employee 
                { 
                Username = currentEmployee.Username,
                Email = currentEmployee.Email,
                Phone = currentEmployee.Phone
                };

                foreach (var taskId in currentEmployee.Tasks.Distinct())
                {
                    if (context.Tasks.All(t=>t.Id != taskId))
                    {
                        output.AppendLine(ErrorMessage);continue;
                    }

                    var task = context.Tasks.FirstOrDefault(t => t.Id == taskId);

                    var tasklist = new EmployeeTask 
                    { 
                    
                       Employee = employee,
                       Task = task
                    
                    };

                    employee.EmployeesTasks.Add(tasklist);
                }

                employees.Add(employee);
                output.AppendLine(String.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();
            return output.ToString().TrimEnd();


        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}