using AutoMapper;
using CarDealer.Data;
using CarDealer.DataTransferObjects.InputModels;
using CarDealer.DataTransferObjects.OutputModels;
using CarDealer.Models;
using CarDealer.XmlHelp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var supplierXml = File.ReadAllText("Datasets/suppliers.xml");
            ImportSuppliers(context, supplierXml);

            var partXml = File.ReadAllText("Datasets/parts.xml");
            ImportParts(context, partXml);
            
            
            var carXml = File.ReadAllText("Datasets/cars.xml");
            ImportCars(context, carXml);

            var customerXml = File.ReadAllText("Datasets/customers.xml");
            ImportCustomers(context, customerXml);

            var salesXml = File.ReadAllText("Datasets/sales.xml");
            var result = ImportSales(context, salesXml);

            Console.WriteLine(GetSalesWithAppliedDiscount(context));

        }

        //Problem 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml) 
        {
            var xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));
            var textRead = new StringReader(inputXml);
            var suppliersDto = xmlSerializer.Deserialize(textRead) as SupplierInputModel[];

            var suppliers = suppliersDto.Select(s => new Supplier
            {
                Name = s.Name,
                IsImporter = s.IsImporter

            })
            .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            
            return $"Successfully imported {suppliers.Count}";
        }

        //Problem 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml) 
        {
            const string root = "Parts";

            var partDto = XmlConverter.Deserializer<PartInputModel>(inputXml, root);

            var suplliersIds = context.Suppliers.Select(s => s.Id).ToList();

            var parts = partDto.Where(p=>suplliersIds.Contains(p.SupplierId))
                .Select(p=> new Part 
                 {  
                      Name = p.Name,
                      Price = p.Price,
                      Quantity = p.Quantity,
                      SupplierId  = p.SupplierId
                  
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}";
        }

        //Problem 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml) 
        {
            const string root = "Cars";
           

            var xmlSerializer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);
            var carsDtos = xmlSerializer.Deserialize(textRead) as CarInputModel[];

            var DbParts = context.Parts.Select(p => p.Id).ToList();

            var cars = carsDtos.Select(c => new Car
            {
                Make = c.Make,
                Model = c.Model,
                TravelledDistance = c.TraveledDistance,
                PartCars = c.PartsIds.Select(p => p.Id)
                                     .Distinct()
                                     .Intersect(DbParts)
                                     .Select(pc => new PartCar 
                                                    {
                                                      
                                                       PartId = pc
                                                    })
                                     .ToList()



            })
                .ToList(); 

            // var cars = new List<Car>();
            //foreach (var currentCarDto in carsDtos)
            //{
            //    var uniquePartsIds = currentCarDto.PartsIds.Select(p => p.Id).Distinct();
            //    var currentCarParts = uniquePartsIds.Intersect(DbParts);
            //
            //    var currentCar = new Car
            //    {
            //        Make = currentCarDto.Make,
            //        Model = currentCarDto.Model,
            //        TravelledDistance = currentCarDto.TraveledDistance,
            //    };
            //
            //    foreach (var part in currentCarParts)
            //    {
            //        var partCar = new PartCar 
            //        { 
            //        PartId = part
            //        
            //        };
            //
            //        currentCar.PartCars.Add(partCar);
            //    }
            //
            //    cars.Add(currentCar);
            //}

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Problem 12. Import Customers

        public static string ImportCustomers(CarDealerContext context, string inputXml) 
        {
            string root = "Customers";
            
            
            InitializeMapper();

            var xmlSerializer = new XmlSerializer(typeof(CustomerInputModel[]),new XmlRootAttribute( root));
            var textRead = new StringReader(inputXml);
            var customersDtos = xmlSerializer.Deserialize(textRead) as CustomerInputModel[];

            var customers = mapper.Map<Customer[]>(customersDtos);

            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}";
        }


        //Problem 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml) 
        {
            string root = "Sales";

            InitializeMapper();
            var xmlSerializer = new XmlSerializer(typeof(SaleInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);
            var saleDto = xmlSerializer.Deserialize(textRead) as SaleInputModel[];

            var carsIds = context.Cars.Select(c => c.Id).ToList();
            var salesDto = saleDto.Where(c => carsIds.Contains(c.CarId))
                               .Select(c => new Sale
                               {
                                   CarId = c.CarId,
                                   CustomerId = c.CustomerId,
                                   Discount = c.Discount
                               })
                               .ToList();
           var sales =  mapper.Map<Sale[]>(salesDto);
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";

        }

        //Problem 14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {

            var cars = context.Cars.Where(c => c.TravelledDistance > 2_000_000)
                                   .Select(c => new CarOutputModel
                                   {
                                       Make = c.Make,
                                       Model = c.Model,
                                       TraveledDistance = c.TravelledDistance
                                   })
                                   .OrderBy(c => c.Make)
                                   .ThenBy(c => c.Model)
                                   .Take(10)
                                   .ToArray();
            string root = "cars";
            XmlSerializer serializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();
           
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, cars, ns);
            
          
            var result = textwriter.ToString();

            return result;
        }

        

        //Problem 15. Export Cars From Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context) 
        {
            var cars = context.Cars.Where(c => c.Make == "BMW")
                                   .Select(c=> new BmwOutputModel 
                                   { 
                                   Id = c.Id,
                                   Model= c.Model,
                                   TravelledDistance = c.TravelledDistance
                                   })
                                   .OrderBy(c=>c.Model)
                                   .ThenByDescending(c=>c.TravelledDistance)
                                   .ToArray();

            string root = "cars";
            XmlSerializer serializer = new XmlSerializer(typeof(BmwOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, cars, ns);
           

            var result = textwriter.ToString();
            File.WriteAllText( "../../../result. txt", result);
            return result;

        }

        //Problem 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context) 
        {
            var suppliers = context.Suppliers.Where(s => s.IsImporter == false)
                                             .Select(s=> new SupplierOutputModel
                                             { 
                                                 Id = s.Id,
                                                 Name = s.Name,
                                                 PartsCout = s.Parts.Count
                                             
                                             }) 
                                             .ToArray();
            string root = "suppliers";
            XmlSerializer serializer = new XmlSerializer(typeof(SupplierOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, suppliers, ns);
            var result = textwriter.ToString();
            File.WriteAllText("../../../result. txt", result);
            return result;
        }

        //Problem 17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context) 
        {
            var carParts = context.Cars
                    .Select(c => new CarPartOutputModel
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance,
                        Parts = c.PartCars.Select(p => new PartsInfo
                        {
                            PartName = p.Part.Name,
                            Price = p.Part.Price
                        }).OrderByDescending(p => p.Price).ToArray()

                    })
                    .OrderByDescending(c => c.TravelledDistance).ThenBy(c => c.Model).Take(5).ToArray();

            string root = "cars";
            XmlSerializer serializer = new XmlSerializer(typeof(CarPartOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, carParts, ns);
            var result = textwriter.ToString();
           // File.WriteAllText("../../../result. txt", result);
            return result;


        }

        //Problem 18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context) 
        {
            var customers = context.Customers.Where(c => c.Sales.Count > 0)
                                             .Select(c => new CustomerOutputModel
                                             {
                                                 FullName = c.Name,
                                                 BoughtCars = c.Sales.Count,
                                                 SpentMoney = c.Sales.Select(p=>p.Car)
                                                                     .SelectMany(p=>p.PartCars)
                                                                     .Sum(p=>p.Part.Price)
                                             })
                                             .OrderByDescending(m=>m.SpentMoney)
                                             .ToArray();

            string root = "customers";
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, customers, ns);
            var result = textwriter.ToString();
            // File.WriteAllText("../../../result. txt", result);
            return result;

        }

        //Problem 19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var suppliers = context.Sales
                                   .Select(s => new SalesDiscountOutputModel
                                   {
                                       Car = new CarSaleOutputModel
                                       {
                                           Make = s.Car.Make,
                                           Model = s.Car.Model,
                                           TraveledDistance = s.Car.TravelledDistance,
                                           
                                       },

                                       Discount = s.Discount,
                                       CustomerName = s.Customer.Name,
                                       Price = s.Car.PartCars.Sum(p => p.Part.Price),
                                       PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) - ((s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100))

                                   }).ToArray();

            string root = "sales";
            XmlSerializer serializer = new XmlSerializer(typeof(SalesDiscountOutputModel[]), new XmlRootAttribute(root));
            var textwriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textwriter, suppliers, ns);
            var result = textwriter.ToString();
            // File.WriteAllText("../../../result. txt", result);
            return result;

        }
        private static void InitializeMapper() 
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<CarDealerProfile>();
               
            });

            mapper = config.CreateMapper();
        }
    }

    
}