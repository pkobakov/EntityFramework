using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class OfficerPrisoner
    {
        //PrisonerId – integer, Primary Key
        //Prisoner – the officer’s prisoner(required)
        //OfficerId – integer, Primary Key
        //Officer – the prisoner’s officer(required)
      
        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }
        
        [ForeignKey(nameof(Officer))]
        public int OfficerId { get; set; }
        public Officer Officer { get; set; }
    }
}