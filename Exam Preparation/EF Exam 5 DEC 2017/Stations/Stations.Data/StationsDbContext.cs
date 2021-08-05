using Microsoft.EntityFrameworkCore;
using Stations.Models;

namespace Stations.Data
{

    public class StationsDbContext : DbContext
    {
        public StationsDbContext()
        {
        }

        public StationsDbContext(DbContextOptions options)
            : base(options)
        {
        }


        public DbSet<Station> Stations { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TrainSeat> TrainSeats { get; set; }
        public DbSet<SeatingClass> SeatingClasses { get; set; }
        public DbSet<CustomerCard> CustomerCards { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Station>(station =>
            {
                station.Property(s => s.Name).IsRequired();
                station.HasIndex(s => s.Name).IsUnique(true);
            });

            builder.Entity<Train>(train=>
            {
                train.HasIndex(t => t.TrainNumber).IsUnique();
            });

            builder.Entity<SeatingClass>(seatingClass => 
            {
                seatingClass.HasIndex(sc => sc.Name).IsUnique();
                seatingClass.Property(a => a.Abbreviation).HasColumnType("CHAR(2)").IsRequired();
                seatingClass.HasIndex(sc => sc.Abbreviation).IsUnique();
            });

            builder.Entity<TrainSeat>(trainSeat=> 
            {
                trainSeat.HasOne(ts=>ts.Train).WithMany(st=>st.TrainSeats).HasForeignKey(ts=>ts.TrainId);
                trainSeat.HasOne(ts => ts.SeatingClass).WithMany(sc => sc.TrainSeats).HasForeignKey(ts=>ts.SeatingClassId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Trip>(trip=> 
            {
                trip.HasOne(t => t.OriginStation).WithMany(os => os.TripsFrom).HasForeignKey(s => s.OriginStationId).OnDelete(DeleteBehavior.Restrict);
                trip.HasOne(t => t.DestinationStation).WithMany(ds => ds.TripsTo).HasForeignKey(t => t.DestinationStationId).OnDelete(DeleteBehavior.Restrict);
                trip.HasOne(t => t.Train).WithMany(tr => tr.Trips).HasForeignKey(t => t.TrainId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}