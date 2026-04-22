using BusBooking.Backend.Models;

namespace BusBooking.Backend.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
