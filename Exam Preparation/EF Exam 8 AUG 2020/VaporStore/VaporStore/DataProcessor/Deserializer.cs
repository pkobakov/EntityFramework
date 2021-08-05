namespace VaporStore.DataProcessor
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
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			// var data = JsonConvert.DeserializeObject(jsonString);
			//foreach( var element in data)
			//var output = new StringBuilder();
			// if (!IsValid(element)) => output.AppendLine("Invalid Data");continue;!!!!!!!!!!!
			//context.AddRange(data)
			//context.SaveChanges();
			//output.AppendLine("Successful import....")

			var importedGames = JsonConvert.DeserializeObject<List<ImportGameModel>>(jsonString);

			var output = new StringBuilder();
            foreach (var importedGame in importedGames)
            {
                if (!IsValid(importedGame)|| importedGame.Tags.Count == 0)
                {
					output.AppendLine("Invalid Data");continue;
                }
				//If a developer/genre/tag with that name doesn’t exist, create it. 
				var genre = context.Genres.FirstOrDefault(g => g.Name == importedGame.Genre) 
					?? new Genre {Name = importedGame.Genre };
				var developer = context.Developers.FirstOrDefault(d => d.Name == importedGame.Developer)
					?? new Developer { Name = importedGame.Developer};
				var game = new Game
				{
					Name = importedGame.Name,
					Price = importedGame.Price,
					ReleaseDate = importedGame.ReleaseDate.Value,
					// for datetime parse variant the import property for the date should be string 
					//ReleaseDate = DateTime.Parse(importedGame.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
					Developer = developer,
					Genre = genre
				};
                foreach (var theTag in importedGame.Tags)
                {
					var tag = context.Tags.FirstOrDefault(t => t.Name == theTag)
						?? new Tag { Name = theTag};
					game.GameTags.Add(new GameTag { Tag = tag});
                }

				context.Games.Add(game);
				context.SaveChanges();

				output.AppendLine($"Added { game.Name} ({ game.Genre.Name}) with {game.GameTags.Count} tags");
            }

			return output.ToString().Trim();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var users = JsonConvert.DeserializeObject<List<ImportUsersModel>>(jsonString);
			var output = new StringBuilder();

            foreach (var user in users)
            {
                if (!IsValid(user)|| !IsValid(user.Cards))
                {
					output.AppendLine("Invalid Data"); continue;
				}

				var newUser = new User
				{
					FullName = user.FullName,
					Username = user.Username,
					Email = user.Email,
					Age = user.Age,
					Cards = user.Cards.Select(c=> new Card 
					{ 
					Number = c.Number,
					Cvc = c.CVC,
					Type = c.Type.Value
					})
					.ToList()
				};
				context.Users.Add(newUser);
				context.SaveChanges();
				output.AppendLine($"Imported {newUser.Username} with {newUser.Cards.Count} cards");
            }
			return output.ToString().Trim();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			string root = "Purchases";
			var serializer = new XmlSerializer(typeof(ImportPurchaseModel[]), new XmlRootAttribute(root));
			var textRead = new StringReader(xmlString);
			var purchases = serializer.Deserialize(textRead) as ImportPurchaseModel[];

			var output = new StringBuilder();

            foreach (var purchaseXml in purchases)
            {
                if (!IsValid(purchaseXml))
                {
					output.AppendLine("Invalid Data"); continue;
				}

				//before to add the date need to parse it first!!!!!!!!!
				var parsedDate = DateTime.ParseExact(purchaseXml.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);


				var purchase = new Purchase
				{
					Type = purchaseXml.Type.Value,
					ProductKey = purchaseXml.Key,
					//here we add the parsed date
					Date = parsedDate,
					Card = context.Cards.FirstOrDefault(c => c.Number == purchaseXml.Card),
				    Game = context.Games.FirstOrDefault(g => g.Name == purchaseXml.Title)
			    };
				


				context.Purchases.Add(purchase);
				context.SaveChanges();
				output.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

			
			return output.ToString().Trim();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}