using wibix_api.Models;
using Microsoft.AspNetCore.Identity;

namespace wibix_api.Repositories;

public interface IAccountRepository{

    /// <summary>
    /// Maps all users to a <see cref="wibix_api.Models.VisibleInfo"/> object and returns them in a list
    /// </summary>
    IEnumerable<VisibleInfo> GetUsers();

    /// <summary>
    /// Gets the <see cref="wibix_api.Models.VisibleInfo"/> of a user with specified
    /// <paramref name="id">id</paramref>
    /// </summary>
    Task<User> GetUser(string id);

    /// <summary>
    /// Creates a <see cref="wibix_api.Models.User"/> object from
    /// <paramref name="model">model</paramref> and adds it to the database
    /// </summary>
    Task<IdentityResult> Register(UserRegister model);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.User"/> with specified
    /// <paramref name="UserName">UserName</paramref> in <see cref="wibix_api.Models.UserLogin"/> validates and return an object with the corresponding jwt token and <see cref="wibix_api.Models.VisibleInfo"/> of the user
    /// </summary>
    Task<Object> Login(UserLogin model);

    /// <summary>
    /// Saves <see cref="Microsoft.AspNetCore.Http.IFormFile"/> in a local directory, maps 
    /// <see cref="wibix_api.Models.UserUpdate"/>  to <see cref="wibix_api.Models.User"/> and updates database
    /// </summary>
    Task UpdateProfile(UserUpdate model);

    /// <summary>
    /// Removes the <see cref="wibix_api.Models.User"/> with specified
    /// <paramref name="id">id</paramref> from the database
    /// </summary>
    Task DeletProfile(string id);
}