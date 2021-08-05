namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var output = new StringBuilder();
            var projects = context.Projects
                                  .Where(p => p.Tasks.Count >= 1)
                                  .ToList()
                                  .Select(p => new ExportProjectModel
                                  {
                                      TasksCount = p.Tasks.Count,
                                      ProjectName = p.Name,
                                      HasEndDate = p.DueDate == null ? "No" : "Yes",
                                      Tasks = p.Tasks.Select(t => new ExportTaskModel
                                      {
                                      Name = t.Name,
                                      Label = t.LabelType.ToString()
                                      
                                      }).OrderBy(t=>t.Name).ToList()
                                  }).OrderByDescending(p=>p.TasksCount).ThenBy(p=>p.ProjectName).ToList();

            string root = "Projects";
            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportProjectModel>), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, String.Empty);
            serializer.Serialize(textwriter, projects, ns);

            var result = textwriter.ToString().Trim();

            return result;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
              .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
              .ToArray()
              .Select(e => new
              {
                  Username = e.Username,
                  Tasks = e.EmployeesTasks
                      .Where(et => et.Task.OpenDate >= date)
                      .OrderByDescending(et => et.Task.DueDate)
                      .ThenBy(et => et.Task.Name)
                      .Select(et => new
                      {
                          TaskName = et.Task.Name,
                          OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                          DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                          LabelType = et.Task.LabelType.ToString(),
                          ExecutionType = et.Task.ExecutionType.ToString()
                      })
                      .ToArray()

              })
              .OrderByDescending(e => e.Tasks.Length)
              .ThenBy(e => e.Username)
              .Take(10)
              .ToArray();

            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }
    }
}