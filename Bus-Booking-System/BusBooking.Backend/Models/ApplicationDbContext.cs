using Microsoft.EntityFrameworkCore;

namespace BusBooking.Backend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<OperatorBoardingPoint> OperatorBoardingPoints { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusRouteChangeRequest> BusRouteChangeRequests { get; set; }
        public DbSet<BusReactivationRequest> BusReactivationRequests { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MockMailMessage> MockMailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OperatorStatus>();
            modelBuilder.HasPostgresEnum<BusStatus>();
            modelBuilder.HasPostgresEnum<BookingStatus>();
            modelBuilder.HasPostgresEnum<PaymentStatus>();
            modelBuilder.HasPostgresEnum<RequestStatus>();

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

            modelBuilder.Entity<City>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<OperatorBoardingPoint>()
                .HasIndex(p => new { p.OperatorId, p.CityId, p.Label })
                .IsUnique();
            modelBuilder.Entity<OperatorBoardingPoint>()
                .HasOne(p => p.Operator)
                .WithMany()
                .HasForeignKey(p => p.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OperatorBoardingPoint>()
                .HasOne(p => p.City)
                .WithMany(c => c.BoardingPoints)
                .HasForeignKey(p => p.CityId)
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
                .WithMany(o => o.Buses)
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Bus>()
                .Property(b => b.Price)
                .HasColumnType("numeric");
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.SourceBoardingPoint)
                .WithMany()
                .HasForeignKey(b => b.SourceBoardingPointId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.DestinationBoardingPoint)
                .WithMany()
                .HasForeignKey(b => b.DestinationBoardingPointId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BusRouteChangeRequest>()
                .HasIndex(r => new { r.BusId, r.Status });
            modelBuilder.Entity<BusRouteChangeRequest>()
                .HasOne(r => r.Bus)
                .WithMany()
                .HasForeignKey(r => r.BusId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BusRouteChangeRequest>()
                .HasOne(r => r.Operator)
                .WithMany()
                .HasForeignKey(r => r.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BusReactivationRequest>()
                .HasIndex(r => new { r.BusId, r.Status });
            modelBuilder.Entity<BusReactivationRequest>()
                .HasOne(r => r.Bus)
                .WithMany()
                .HasForeignKey(r => r.BusId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BusReactivationRequest>()
                .HasOne(r => r.Operator)
                .WithMany()
                .HasForeignKey(r => r.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Seats
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.BusId, s.SeatNumber })
                .IsUnique();
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
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

            // Configure Mock Mail Messages
            modelBuilder.Entity<MockMailMessage>()
                .HasIndex(m => new { m.ToEmail, m.CreatedAt });
            modelBuilder.Entity<MockMailMessage>()
                .HasIndex(m => new { m.ToEmail, m.IsRead });
            modelBuilder.Entity<MockMailMessage>()
                .HasOne(m => m.ParentMessage)
                .WithMany()
                .HasForeignKey(m => m.ParentMessageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
