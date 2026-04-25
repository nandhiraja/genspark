
namespace BusBooking.Backend.Models
{
    public enum UserRole       { USER, OPERATOR, ADMIN }
    public enum OperatorStatus { PENDING, APPROVED, REJECTED }
    public enum BusStatus      { ACTIVE, INACTIVE, PENDING_APPROVAL }
    public enum BookingStatus  { PENDING, CONFIRMED, CANCELLED }
    public enum PaymentStatus  { INITIATED, SUCCESS, FAILED }
    public enum RequestStatus  { PENDING, APPROVED, REJECTED }
}