namespace BusBooking.Backend.Models
{
    public enum UserRole { USER, OPERATOR, ADMIN }
    public enum OperatorStatus { PENDING, APPROVED, REJECTED }
    public enum BusStatus { ACTIVE, INACTIVE }
    public enum BookingStatus { PENDING, CONFIRMED, CANCELLED }
    public enum PaymentStatus { INITIATED, SUCCESS, FAILED }
}
