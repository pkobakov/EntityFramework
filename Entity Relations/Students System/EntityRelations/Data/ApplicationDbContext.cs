using EntityRelations.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityRelations.Data
{
    public class ApplicationDbContext : DbContext
    {
        private const string ConnectionString = "Server= .; Database= StudentsSystem; Integrated Security = true;";
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
 
        }

        public DbSet<Course> Courses { get; set; }  
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasOne(s => s.Student)
                      .WithMany(h => h.Homeworks)
                      .HasForeignKey(s => s.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Course)
                      .WithMany(h => h.Homeworks)
                      .HasForeignKey(c => c.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StudentCourse>(entity => 
            {
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });
                entity.HasOne(s=>s.Student)
                      .WithMany (c=>c.StudentCourses)
                      .HasForeignKey(s=>s.StudentId);
                entity.HasOne(c => c.Course)
                      .WithMany(s => s.CourseStudents)
                      .HasForeignKey(c => c.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
                

            
            base.OnModelCreating(modelBuilder);
        }
    }
}
