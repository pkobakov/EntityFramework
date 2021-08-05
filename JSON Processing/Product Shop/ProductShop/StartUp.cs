using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var productShopcontext = new ProductShopContext();
            productShopcontext.Database.EnsureDeleted();
            productShopcontext.Database.EnsureCreated();


            string usersInputJSON = File.ReadAllText("../../../Datasets/users.json");
            //var result = ImportUsers(productShopcontext, usersInputJSON);
            
            string productsInputJSON = File.ReadAllText("../../../Datasets/products.json");
            
            ImportUsers(new ProductShopContext(), usersInputJSON);
            
            //var result = ImportProducts(productShopcontext, productsInputJSON);
            
            ImportProducts(productShopcontext, productsInputJSON);
            
            string categoriesInputJSON = File.ReadAllText("../../../Datasets/categories.json");
            
            //var result = ImportCategories(productShopcontext, categoriesInputJSON);
            
            ImportCategories(productShopcontext, categoriesInputJSON);
            
            string categoriesProductInputJSON = File.ReadAllText("../../../Datasets/categories-products.json");

            //var result = ImportCategoryProducts(productShopcontext, categoriesProductInputJSON);
            //Console.WriteLine(result);

            ImportCategoryProducts(new ProductShopContext(), categoriesProductInputJSON);

            var result = GetUsersWithProducts(new ProductShopContext());


            Console.WriteLine(GetUsersWithProducts(new ProductShopContext()));

        }

        

        //Problem 2. Import Users
        public static string ImportUsers(ProductShopContext context, string usersInputJSON) 
        {
            InitializeAutoMapper();
            var dtoUsers = JsonConvert.DeserializeObject<List<UserInputDto>>(usersInputJSON);
            var users = mapper.Map<List<User>>(dtoUsers);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count}";
        }

        //Problem 3. Import Products
        public static string ImportProducts(ProductShopContext context, string productsInputJSON) 
        {
            InitializeAutoMapper();
            var dtoProducts = JsonConvert.DeserializeObject<List<ProductInputDto>>(productsInputJSON);
            var products = mapper.Map<List<Product>>(dtoProducts);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 4. Import Categories
        public static string ImportCategories(ProductShopContext context, string categoriesInputJSON) 
        {
            InitializeAutoMapper();
            var dtoCategories = JsonConvert.DeserializeObject<List<CategoriesInputDto>>(categoriesInputJSON).Where(c=>c.Name!=null).ToList();
            var categories = mapper.Map<List<Category>>(dtoCategories);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        
        }

        //Problem 5. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string categoriesProductsinputJSONinputJson)
        {
            InitializeAutoMapper();
            var categoriesProductsDto = JsonConvert.DeserializeObject<List<CategoriesProductsDto>>(categoriesProductsinputJSONinputJson);
            var categoriesProducts = mapper.Map<List<CategoryProduct>>(categoriesProductsDto);
            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

        return $"Successfully imported {categoriesProducts.Count}";
        }

        //Problem 6. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context) 
        {
            var products = context.Products
                                  .Where(p => p.Price >= 500 && p.Price <= 1000)
                                  .Select(p => new
                                  {

                                      name = p.Name,
                                      price = p.Price,
                                      seller = p.Seller.FirstName + " " + p.Seller.LastName

                                  })
                                  .OrderBy(p => p.price)
                                  .ToList();

            var productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);
            return productsJson;
        
        }

        //Problem 7. Export Successfully Sold Products

        public static string GetSoldProducts(ProductShopContext context) 
        {

            var users = context.Users
                               .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
                               .Select(user => new
                               {
                                   firstName = user.FirstName,
                                   lastName = user.LastName,
                                   soldProducts = user.ProductsSold.Where(p => p.BuyerId != null)
                                                                   .Select(b => new
                                                                   {

                                                                       name = b.Name,
                                                                       price = b.Price,
                                                                       buyerFirstName = b.Buyer.FirstName,
                                                                       buyerLastName = b.Buyer.LastName

                                                                   })
                                                                   .ToList()
                               })
                               .OrderBy(user=>user.lastName)
                               .ThenBy(user=>user.firstName)
                               .ToList();

            return JsonConvert.SerializeObject(users,Formatting.Indented);
        
        }

        //Problem 8. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context) 
        {
            var count = context.Categories
                               .Select(c => new
                               {
                                   category = c.Name,
                                   productsCount = c.CategoryProducts.Count,
                                   averagePrice =c.CategoryProducts.Count == 0? 0.ToString("F2"): c.CategoryProducts.Average(p => p.Product.Price).ToString("F2"),
                                   totalRevenue = c.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")
                               })
                               .OrderBy(c => c.productsCount)
                               .ToList();

        return JsonConvert.SerializeObject(count, Formatting.Indented);
        }

       //Problem 9. Export Users and Products
       public static string GetUsersWithProducts(ProductShopContext context) 
       {

            var users = context.Users.Include(u=>u.ProductsSold)
                                     .ToList()
                                     .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
                                     .Select(u => new
                                     {
                                    
                                         firstName = u.FirstName,
                                         lastName = u.LastName,
                                         age = u.Age,
                                         soldProducts = new
                                         {
                                             count = u.ProductsSold.Where(p=>p.BuyerId!=null).Count(),
                                             products = u.ProductsSold.Where(p=>p.BuyerId!=null)
                                             .Select(o => new
                                             {
                                                 name = o.Name,
                                                 price = o.Price
                                             })
                                         }
                                     })
                                     .OrderByDescending(u=>u.soldProducts.products.Count())
                                     .ToList();

            var resultObj = new
            {

                usersCount = context.Users.Where(u=>u.ProductsSold.Any(b=>b.BuyerId!=null)).Count(),
                users = users
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore

            };

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);
       }

        private static void InitializeAutoMapper()
        {
            var mapconfig = new MapperConfiguration(config =>
            {
                config.AddProfile<ProductShopProfile>();
            });

            mapper = mapconfig.CreateMapper();
        }
    }
}