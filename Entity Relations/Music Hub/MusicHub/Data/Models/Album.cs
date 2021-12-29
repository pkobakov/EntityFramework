using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MusicHub.Data.Models
{
    
    public class Album
    {
        public Album()
        {
            Songs = new HashSet<Song>();
        }
        public int Id { get; set; }
        public string Name  { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get => Songs.Sum(s => s.Price);}
        public int? ProducerId { get; set; }
        public Producer Producer { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
}
