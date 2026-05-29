using BankingAPI.Models.DTOs;

namespace BankingAPI.Interfaces
{
    public interface IAuthenticationService
    {
        public RegisterUserResponse Register(RegisterUserRequest reques);
        public LoginResponse Login(LoginRequest request);
    }
}
