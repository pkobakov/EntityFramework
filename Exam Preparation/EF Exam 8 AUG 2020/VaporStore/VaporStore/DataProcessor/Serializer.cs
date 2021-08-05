namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;
    using VaporStore.ExportResults;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context.Genres
				.ToList()
				.Where(genre => genreNames.Contains(genre.Name))
				.Select(genre => new
				{
					Id = genre.Id,
					Genre = genre.Name,
					Games = genre.Games.Select(game => new
					{

						Id = game.Id,
						Title = game.Name,
						Developer = game.Developer.Name,
						Tags = string.Join(", ", game.GameTags.Select(tag => tag.Tag.Name)),
						Players = game.Purchases.Count,
					})
					
					.Where(game => game.Players > 0)
					.OrderByDescending(game => game.Players)
					.ThenBy(game=>game.Id),

					TotalPlayers = genre.Games.Sum(p=>p.Purchases.Count),
				})
				.OrderByDescending(genre=>genre.TotalPlayers)
				.ThenBy(genre=>genre.Id)
				.ToList();

				
			return JsonConvert.SerializeObject(genres,Formatting.Indented);
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{

			var data = context.Users.ToList().Where(c => c.Cards.Any(p => p.Purchases.Where(t=>t.Type.ToString()==storeType).Any())).ToList()
									.Select(u => new ExportUserXml
									{
										UserName = u.Username,
										TotalSpent = u.Cards.Sum(c => c.Purchases.Where(t=>t.Type.ToString()==storeType).Sum(s => s.Game.Price)),
										Purchases = u.Cards.SelectMany(p => p.Purchases).Where(p=>p.Type.ToString() == storeType)
														   .Select(g => new ExportPurchaseModel
														   {
															   CardNumber = g.Card.Number,
															   Cvc = g.Card.Cvc,
															   Date = g.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
															   Game = new ExportGameXml
															   {
																   Name = g.Game.Name,
																   Genre = g.Game.Genre.Name,
																   Price = g.Game.Price
															   }
														   }).OrderBy(p=>p.Date).ToArray()

									}).OrderByDescending(u => u.TotalSpent).ThenBy(u => u.UserName).ToArray();





			string root = "Users";
			XmlSerializer serializer = new XmlSerializer(typeof(ExportUserXml[]), new XmlRootAttribute(root));
			var textwriter = new StringWriter();

			var ns = new XmlSerializerNamespaces();
			ns.Add(String.Empty, String.Empty);
			serializer.Serialize(textwriter, data, ns);

			var result = textwriter.ToString().Trim();

			return result;
		}
	}
}