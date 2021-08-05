namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using AutoMapper;
    using MusicHub.Data.Models;
    using Newtonsoft.Json;
    using System.Reflection;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

           //DbInitializer.ResetDatabase(context);

            //var exportAlbumsInfoResult = ExportAlbumsInfo(context, 9);
            //File.WriteAllText("../../../exportAlbumsResult",exportAlbumsInfoResult);
            //
            //var aboveDurationResult = ExportSongsAboveDuration(context, 4);
            //File.WriteAllText("../../../aboveDurationResult.txt", aboveDurationResult);

            var mapconfig = new MapperConfiguration(mapconfig =>
                                                             {

                                                                 mapconfig.CreateMap<SongPerformer, SongInfoDto>()
                                                                 .ForMember(d=>d.Performer,op=>op.MapFrom(p=>p.Performer.FirstName));
                                                             });
            var mapper = mapconfig.CreateMapper();
         

           
            Console.WriteLine(mapper);
            

            
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumInfo = context.Producers.FirstOrDefault(p => p.Id == producerId)
                                   .Albums
                                   .Select(a => new
                                   {
                                       Name = a.Name,
                                       ReleaseDate = a.ReleaseDate,
                                       ProducerName = a.Producer.Name,
                                       Songs = a.Songs.Select(s => new
                                       {
                                           SongName = s.Name,
                                           Price = s.Price,
                                           SongWriter = s.Writer.Name
                                           
                                       })
                                       .OrderByDescending(s=>s.SongName)
                                       .ThenBy(s=>s.SongWriter)
                                       .ToList(),
                                       AlbumPrice = a.Price
                                   }) 
                                .OrderByDescending(a=>a.AlbumPrice)
                                .ToList();

            var sb = new StringBuilder();

            foreach (var album in albumInfo.OrderByDescending(a=>a.AlbumPrice))
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) }");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                int counter = 0; 

                foreach (var song in album.Songs)
                {
                    counter++;
                    sb.AppendLine($"---#{counter}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:F2}");

            }

            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs.ToList()
                              .Where(s => s.Duration.TotalSeconds > duration)
                              .Select(s => new
                              {
                                  SongName = s.Name,
                                  WriterName = s.Writer.Name,
                                  PerformerFullName = s.SongPerformers
                                                       .Select(s =>$"{s.Performer.FirstName} {s.Performer.LastName}")
                                                       .FirstOrDefault(),
                                  AlbumProducer = s.Album.Producer.Name,
                                  Duration = s.Duration

                              })
                           .OrderBy(s => s.SongName)
                           .ThenBy(s => s.WriterName)
                           .ThenBy(s => s.PerformerFullName)
                           .ToList();


            var sb = new StringBuilder();
            int counter = 1;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}"); 
                sb.AppendLine($"---Performer: {song.PerformerFullName}"); 
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");

                counter++;
            }

            return sb.ToString().Trim();
        }

     
    }

    class SongInfoDto 
    {

        public string Name { get; set; }
        public string Performer { get; set; }


    }
}
