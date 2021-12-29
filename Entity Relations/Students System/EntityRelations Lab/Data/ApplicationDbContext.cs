using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityRelations_Lab.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=ERLabDatabase; Integrated Security=true;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Employee>().HasKey(x => x.EmployeeId);
            modelBuilder.Entity<Employee>().Property(x => x.Name).HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Lastname).HasMaxLength(20).IsRequired();  
            

            base.OnModelCreating(modelBuilder);
        }
    }
}

