using BusBooking.Backend.DTOs;
using BusBooking.Backend.Helpers;
using BusBooking.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<UserInfoDto?> RegisterAsync(RegisterRequestDto request)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (exists) return null;

            var user = new User
            {
                Name         = request.Name,
                Email        = request.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone        = request.Phone,
                Role         = UserRole.USER
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return MapToUserInfo(user);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponseDto
            {
                Token  = token,
                Name   = user.Name,
                Email  = user.Email,
                Role   = user.Role.ToString(),
                UserId = user.Id
            };
        }

        public async Task<UserInfoDto?> GetCurrentUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user == null ? null : MapToUserInfo(user);
        }

        private static UserInfoDto MapToUserInfo(User user) => new()
        {
            Id           = user.Id,
            Name         = user.Name,
            Email        = user.Email,
            Role         = user.Role.ToString(),
            Phone        = user.Phone,
            ProfileImage = user.ProfileImage
        };
    }
}