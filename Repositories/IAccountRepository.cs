using wibix_api.Models;
using Microsoft.AspNetCore.Identity;

namespace wibix_api.Repositories;

public interface IAccountRepository{
    IEnumerable<VisibleInfo> GetUsers();
    Task<User> GetUser(string id);
    Task<IdentityResult> Register(UserRegister model);
    Task<Object> Login(UserLogin model);
    Task UpdateProfile(UserUpdate model);
    Task DeletProfile(string id);
}