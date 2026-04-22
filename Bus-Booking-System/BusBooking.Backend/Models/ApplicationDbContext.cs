using Microsoft.EntityFrameworkCore;

namespace BusBooking.Backend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OperatorStatus>();
            modelBuilder.HasPostgresEnum<BusStatus>();
            modelBuilder.HasPostgresEnum<BookingStatus>();
            modelBuilder.HasPostgresEnum<PaymentStatus>();

            // Configure Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure Operators
            modelBuilder.Entity<Operator>()
                .HasIndex(o => o.UserId)
                .IsUnique();
            modelBuilder.Entity<Operator>()
                .HasOne(o => o.User)
                .WithOne()
                .HasForeignKey<Operator>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Routes
            modelBuilder.Entity<Route>()
                .HasIndex(r => new { r.Source, r.Destination })
                .IsUnique();

            // Configure Buses
            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();
            modelBuilder.Entity<Bus>()
                .HasIndex(b => new { b.RouteId, b.StartTime });
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.Operator)
                .WithMany()
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Bus>()
                .Property(b => b.Price)
                .HasColumnType("numeric");

            // Configure Seats
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.BusId, s.SeatNumber })
                .IsUnique();
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany()
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Bookings
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.UserId);
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("numeric");

            // Configure Tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.SeatId)
                .IsUnique()
                .HasFilter("\"BookingId\" IS NOT NULL");

            // Configure SeatLocks
            modelBuilder.Entity<SeatLock>()
                .HasOne(sl => sl.Seat)
                .WithMany()
                .HasForeignKey(sl => sl.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Application-level concurrency check is used for active locks
            // to avoid NOW() immutable function issues in PostgreSQL indexes.

            // Configure Payments
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("numeric");
                
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
