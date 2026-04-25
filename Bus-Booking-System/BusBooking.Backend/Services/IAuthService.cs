using BusBooking.Backend.DTOs;
using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IAuthService
    {
        Task<UserInfoDto?> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<UserInfoDto?> GetCurrentUserAsync(Guid userId);
    }
}