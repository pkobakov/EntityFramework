using System;
using System.Collections.Generic;
using System.Text;

namespace MusicHub.Data.Models
{
   public class Performer
    {
        public Performer()
        {
            PerformerSongs = new HashSet<SongPerformer>();
        }
        public int Id { get; set; }
        public string FirstName  { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public decimal NetWorth { get; set; }
        public virtual ICollection<SongPerformer> PerformerSongs { get; set; }
    }
}
