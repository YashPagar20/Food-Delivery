using FoodDelivery.DTOs;

namespace FoodDelivery.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> registerAsync(RegisterRequest request);
        Task<AuthResponse> loginAsync(LoginRequest request);
    }
}
