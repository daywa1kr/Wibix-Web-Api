using Microsoft.AspNetCore.Identity;
using wibix_api.Models;
using wibix_api.Services;

namespace wibix_api.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private IAuthManager _authManager {get; set;}
    private static IWebHostEnvironment _env{get; set;}=null!;
    public AccountRepository(UserManager<User> userManager, SignInManager<User> signInManager, IAuthManager authManager, IWebHostEnvironment env)
    {
        _userManager=userManager;
        _signInManager=signInManager;
        _authManager=authManager;
        _env=env;
    }
    public async Task DeletProfile(string id)
    {
        var user=await _userManager.FindByIdAsync(id);
        await _userManager.DeleteAsync(user);
    }

    public async Task<User> GetUser(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public IEnumerable<VisibleInfo> GetUsers()
    {
        List<VisibleInfo> users=new List<VisibleInfo>();
        List<User> list=_userManager.Users.OrderBy(u=>u.Rating).ToList();
        list.Reverse();
        foreach (var i in list)
        { 
            users.Add(new VisibleInfo{
                Id=i.Id,
                DisplayName=i.DisplayName,
                UserName=i.UserName,
                Email=i.Email,
                Rating=i.Rating,
                ImageSrc=i.ImageSrc,
                Bio=i.Bio
            });
        }
        return users;
    }

    public async Task<Object> Login(UserLogin model)
    {
        User u=await _userManager.FindByNameAsync(model.UserName);

        if(! await _authManager.ValidateUser(model, u))
        {
            return null!;
        }

        VisibleInfo user=new VisibleInfo{
            Id=u.Id,
            DisplayName=u.DisplayName,
            UserName=u.UserName,
            Email=u.Email,
            Rating=u.Rating,
            ImageSrc=u.ImageSrc,
            Bio=u.Bio
        };

        return (new {Token=await _authManager.CreateToken(u), VisibleInfo=user});
    }

    public async Task<IdentityResult> Register(UserRegister model)
    {
        User u=new User{
            Bio="",
            UserName=model.UserName,
            Email=model.Email,
            DisplayName=model.UserName,
            Roles=new List<string>(){"User"}
        };

        var results= await _userManager.CreateAsync(u, model.Password);

        foreach (var r in u.Roles)
        {
            await _userManager.AddToRoleAsync(u, r);
        }

        return results;
    }

    public async Task UpdateProfile(UserUpdate model)
    {
        if(model.File!=null)
        {
            string fileName=new String(Path.GetFileNameWithoutExtension(model.File.FileName).Take(10).ToArray()).Replace(' ', '-');

            fileName=fileName+DateTime.Now.ToString("yymmssfff")+Path.GetExtension(model.File.FileName);

            string serverFolder=Path.Combine(_env.WebRootPath, "Uploads/", fileName);

            model.File.CopyTo(new FileStream(serverFolder, FileMode.Create));

            var user=await _userManager.FindByIdAsync(model.Id);

            user.ImageSrc=fileName;
            user.Bio=model.Bio;
            user.DisplayName=model.DisplayName;
            user.Email=model.Email;

            await _userManager.UpdateAsync(user);
        }
    }
}