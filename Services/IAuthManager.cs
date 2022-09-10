using wibix_api.Models;

namespace wibix_api.Services;

public interface IAuthManager{

    /// <summary>
    /// Finds the user with the specified username in <paramref name="model"></paramref> and validates the password
    /// </summary>
    Task<bool> ValidateUser(UserLogin model, User user);

    /// <summary>
    /// Returns a jwt token created from <paramref name="user">user</paramref> information
    /// </summary>
    Task<string> CreateToken(User user);
}