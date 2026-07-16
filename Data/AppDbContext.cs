using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailwayReservation.Models;

namespace RailwayReservation.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Train> Trains { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Precision for Currency
            modelBuilder.Entity<Train>().Property(t => t.BaseFare).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalFare).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Train>().Property(t => t.BusinessPercentage).HasPrecision(5, 2);


            // Enforce Unique PNR
            modelBuilder.Entity<Booking>().HasIndex(b => b.PNR).IsUnique();

            // Configure One-to-Many: Booking -> Passengers
            modelBuilder.Entity<Passenger>()
                .HasOne<Booking>()
                .WithMany(b => b.Passengers)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}