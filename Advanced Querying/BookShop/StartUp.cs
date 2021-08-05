namespace BookShop
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using BookShop.Models.Enums;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //Age Restriction:
            //string command = Console.ReadLine();
            //Console.WriteLine(GetBooksByAgeRestriction(db, command));

            //Golden books:
            //Console.WriteLine(GetGoldenBooks(db));

            //Books by Price:
            //Console.WriteLine(GetBooksByPrice(db));

            //Not Released In
            //int year = int.Parse(Console.ReadLine());  
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //Book Titles by Category
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db,input ));

            //Released Before Date
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db,input));

            //Author Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, input));

            //Book Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBookTitlesContaining(db, input));

            //Book Search By Author
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAuthor(db,input));

            //Count Books
            //int input = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(db, input));

            //Total Book Copies
            //Console.WriteLine(CountCopiesByAuthor(db));

            //Profit by Category
            //Console.WriteLine(GetTotalProfitByCategory(db));

            //Most Recent Books
            //Console.WriteLine(GetMostRecentBooks(db));

            //Increase Prices
            //IncreasePrices(db);

            //Remove Books
            //Console.WriteLine(RemoveBooks(db));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var books = context.Books
                           .Where(b => b.AgeRestriction == ageRestriction)
                           .Select(b => b.Title)
                           .OrderBy(t => t)
                           .ToArray();

            var result = string.Join(Environment.NewLine, books);
            return result;

        }
        public static string GetBooksByPrice(BookShopContext context) 
        {

            var booksMoreThan40 = context.Books.Where(b => b.Price > 40)
                                         .Select(b => new { bookTitle = b.Title, bookPrice = b.Price })
                                         .OrderByDescending(b => b.bookPrice)
                                         .ToList();

            var sb = new StringBuilder();

            foreach (var book in booksMoreThan40)
            {
                sb.AppendLine($"{book.bookTitle} - ${book.bookPrice:F2}");
            }

            return sb.ToString().Trim();
        
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year) 
        {
            var booksNotReleasedOnDate = context.Books
                                                .Where(books => books.ReleaseDate.Value.Year != year)
                                                .Select(b => new
                                                {
                                                    bookId = b.BookId,
                                                    bookTitle = b.Title

                                                })
                                                .OrderBy(books=>books.bookId)
                                                .ToArray();

            return string.Join(Environment.NewLine, booksNotReleasedOnDate.Select(b=>b.bookTitle));
        
        
        }
        public static string GetBooksByCategory(BookShopContext context, string input) 
        {
            string[] text = input.Split().Select(s=>s.ToLower()).ToArray();
            var books = context.Books
                             .Where(b => b.BookCategories.Any(c => text.Contains(c.Category.Name.ToLower())))
                             .Select(b => new { 
                                 bTitle = b.Title
                                              })
                             .OrderBy(b => b.bTitle)
                             .ToList();
            return string.Join(Environment.NewLine, books.Select(b=>b.bTitle));
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date) 
        {
            var inputdate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.Where(b => b.ReleaseDate < inputdate)
                               .Select(b => new
                               {
                                   bookTitle = b.Title,
                                   bookEditionType = b.EditionType,
                                   bookPrice = b.Price,
                                   bookReleaseDate = b.ReleaseDate

                               })
                               .OrderByDescending(b => b.bookReleaseDate)
                               .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.bookTitle} - {book.bookEditionType} - ${book.bookPrice:F2}");
            }

            return sb.ToString().Trim();

           // return string.Join(Environment.NewLine, books.Select(b => $"{b.bookTitle} - {b.bookEditionType} - ${b.bookPrice:F2}"));
        
        
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input) 
        {


            var authors = context.Authors.Where(a => a.FirstName.EndsWith(input))
                                    .Select(a => new
                                    {
                                        AuthorFirstName = a.FirstName,
                                        AthorLastName = a.LastName,
                                      
                                    })
                                    .OrderBy(a=>a.AuthorFirstName)
                                    .ThenBy(a=>a.AthorLastName)
                                    .ToArray();

            //var authors = context.Authors.Where(a => EF.Functions.Like(a.FirstName,$"%{input}"))
            //                        .Select(a => new
            //                        {
            //                            AuthorFirstName = a.FirstName,
            //                            AthorLastName = a.LastName,
            //                          
            //                        })
            //                        .OrderBy(a=>a.AuthorFirstName)
            //                        .ThenBy(a=>a.AthorLastName)
            //                        .ToArray();




            return string.Join(Environment.NewLine, authors.Select(b=>$"{b.AuthorFirstName} {b.AthorLastName}"));
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input) 
        {
            var check = input.ToLower();
            var books = context.Books
                               .Where(b =>EF.Functions.Like(b.Title.ToLower(), $"%{check}%"))
                               .Select(b => new
                               {
                                   bookTitle = b.Title
                               })
                               .OrderBy(b=>b.bookTitle)
                               .ToArray();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.bookTitle}"));
        
        }
        public static string GetBooksByAuthor(BookShopContext context, string input) 
        {
            string check = input.ToLower() ;
            var result = context.Books.Where(b => EF.Functions.Like(b.Author.LastName.ToLower(), $"{check}%"))
                                        .Select(b => new
                                        {
                                            AuthorFullName = $"{b.Author.FirstName} {b.Author.LastName}",
                                            BookTitle = b.Title,
                                            AuthorId = b.Author.AuthorId
                                        })
                                        .OrderBy(b=>b.AuthorId)
                                        .ToArray();
            return string.Join(Environment.NewLine, result.Select(r=>$"{r.BookTitle} ({r.AuthorFullName})"));
        }
        public static int CountBooks(BookShopContext context, int lengthCheck) 
        {
            var titleLength = context.Books.Where(b => b.Title.Length > lengthCheck);
                                  
            return titleLength.Count() ;
        
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var output = context.Authors.Select(a => new
            {

                AuthorFullName = $"{a.FirstName} {a.LastName}",
                TotalCopies = a.Books.Sum(b => b.Copies)
            })
            .OrderByDescending(a => a.TotalCopies)
            .ToList();

            return string.Join(Environment.NewLine, output.Select(a=>$"{a.AuthorFullName} - {a.TotalCopies}"));
        }
        public static string GetTotalProfitByCategory(BookShopContext context) 
        {
            var result = context.Categories
                                .Select(c => new
                                {
                                    CategoryName = c.Name,
                                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)

                                })
                                 .OrderByDescending(c => c.Profit)
                                 .ToArray();

            return string.Join(Environment.NewLine, result.Select(c => $"{c.CategoryName} ${c.Profit:F2}")); ;
        }
        public static string GetMostRecentBooks(BookShopContext context) 
        {
            var categories = context.Categories.Select(c => new
                                                              {
                                                                  CategoryName = c.Name,
                                                                  Books = c.CategoryBooks
                                                                              .Select(b=>new 
                                                                                           {
                                                                                               b.Book.Title,
                                                                                              b.Book.ReleaseDate.Value 
                                                                                           })
                                                                              .OrderByDescending(c=>c.Value)
                                                                              .Take(3)
                                                                              .ToList()            
                                                              })
                                                     .OrderBy(c=>c.CategoryName)
                                                     .ToArray();

            var result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Books)
                {
                    result.AppendLine($"{ book.Title} ({book.Value.Year})" );
                }

            }

            return result.ToString().Trim();
        
        }
        public static void IncreasePrices(BookShopContext context) 
        {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010)
                                     .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }
        public static int RemoveBooks(BookShopContext context) 
        {

            var books = context.Books.Where(b => b.Copies < 4200).ToList();

            context.RemoveRange(books);

            context.SaveChanges();

            return books.Count;
        
        }
    }
}
