namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            string root = "Books";
            XmlSerializer serializer = new XmlSerializer(typeof(BookImportModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(xmlString);
            var bookImportModels = serializer.Deserialize(textRead) as BookImportModel[];

            var books = new List<Book>();
            var output = new StringBuilder();

            foreach (var currentBookModel in bookImportModels)
            {
                if (!IsValid(currentBookModel))
                {
                    output.AppendLine(ErrorMessage); continue; //statement breaks one iteration (in the loop), if a specified condition occurs, and continues with the next iteration in the loop.
                }

                DateTime publishedOn;
                bool IsValidDate = DateTime.TryParseExact(currentBookModel.PublishedOn,
                                                                          "MM/dd/yyyy",
                                                                          CultureInfo.InvariantCulture,
                                                                          DateTimeStyles.None,
                                                                          out publishedOn);
                if (!IsValidDate)
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                var validBook = new Book 
                { 
                
                Name = currentBookModel.Name,
                Genre = (Genre) currentBookModel.Genre,
                Pages = currentBookModel.Pages,
                Price = currentBookModel.Price,
                PublishedOn = publishedOn
                };

                books.Add(validBook);
                output.AppendLine(string.Format(SuccessfullyImportedBook, validBook.Name, validBook.Price));
            }

            context.Books.AddRange(books);
            context.SaveChanges();

            return output.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var output = new StringBuilder();
            var authorsImportModel = JsonConvert.DeserializeObject<AuthorImportModel[]>(jsonString);

            var authors = new List<Author>();

            foreach (var currentAuthorModel in authorsImportModel)
            {
                if (!IsValid(currentAuthorModel))
                {
                    output.AppendLine(ErrorMessage);continue;
                }

               

                if (authors.Any(a=>a.Email == currentAuthorModel.Email))
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                var author = new Author() 
                { 
                   FirstName = currentAuthorModel.FirstName,
                   LastName = currentAuthorModel.LastName,
                   Email = currentAuthorModel.Email,
                   Phone = currentAuthorModel.Phone
                };

                foreach (var currentBook in currentAuthorModel.Books)
                {
                    if (!currentBook.BookId.HasValue)
                    {
                        continue;
                    }
                    var testBook = context.Books
                                          .FirstOrDefault(b => b.Id == currentBook.BookId);

                    if (testBook == null)
                    {
                        continue;
                    }

                    author.AuthorsBooks.Add(new AuthorBook 
                    { 
                     Author = author,
                     Book = testBook
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    output.AppendLine(ErrorMessage); continue;
                }

                authors.Add(author);
                output.AppendLine(string.Format(SuccessfullyImportedAuthor,
                    $"{author.FirstName} {author.LastName}", author.AuthorsBooks.Count));

            }

            context.Authors.AddRange(authors);
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