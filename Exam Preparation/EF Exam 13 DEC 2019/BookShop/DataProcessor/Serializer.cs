namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var mostCreaziestAuthors = context.Authors
                                              .Select(a=>new 
                                                             { 
                                                               AuthorName = a.FirstName+' '+a.LastName,
                                                               Books = a.AuthorsBooks
                                                               .OrderByDescending(b=>b.Book.Price)
                                                               .Select(b=>new 
                                                                                                   {
                                                                                                       BookName = b.Book.Name,
                                                                                                       BookPrice = $"{b.Book.Price:F2}"
                                                                                                   })
                                                                                                    .ToArray()
                                                                                                    
                                                              })
                                              .ToArray()
                                              .OrderByDescending(a=>a.Books.Length)
                                              .ThenByDescending(a=>a.AuthorName)
                                              .ToArray();

            var jsonString = JsonConvert.SerializeObject(mostCreaziestAuthors, Formatting.Indented);
            return jsonString;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var oldestBooks = context.Books
                                     .Where(b => b.PublishedOn < date && b.Genre == Data.Models.Enums.Genre.Science)
                                     .ToArray()
                                     .OrderByDescending(b => b.Pages)
                                     .ThenByDescending(b => b.PublishedOn)
                                     .Take(10)
                                     .Select(b => new BookExportModel
                                     {
                                         Name = b.Name,
                                         Date = b.PublishedOn.ToString("d",CultureInfo.InvariantCulture),
                                         Pages = b.Pages
                                     })
                                     .ToArray();

            string root = "Books";
            XmlSerializer serializer = new XmlSerializer(typeof(BookExportModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, oldestBooks, ns);

            var result = textwriter.ToString().Trim();

            return result;
        }
    }
}