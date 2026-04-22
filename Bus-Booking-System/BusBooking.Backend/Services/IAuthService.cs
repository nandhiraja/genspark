using BusBooking.Backend.DTOs;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
