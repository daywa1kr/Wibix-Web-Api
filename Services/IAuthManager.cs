using wibix_api.Models;

namespace wibix_api.Services;

public interface IAuthManager{
    Task<bool> ValidateUser(UserLogin model, User user);
    Task<string> CreateToken(User user);
}