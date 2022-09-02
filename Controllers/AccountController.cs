using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;
using wibix_api.Services;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : Controller{
    private readonly UserManager<User> userManager=null!;
    private readonly SignInManager<User> signInManager=null!;
    private readonly IAuthManager authManager=null!;
    public AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager, IAuthManager _authManager){
        userManager=_userManager;
        signInManager=_signInManager;
        authManager=_authManager;
    }

    [HttpGet]
    public IActionResult GetUsers(){

        return Ok(userManager.Users.ToList());
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegister user){
        if(!ModelState.IsValid)
            return BadRequest();
        
        User u=new User();
        u.Bio="";
        u.UserName=user.UserName;
        u.Email=user.Email;
        u.Password=user.Password;
        u.Roles=new List<string>(){"User"};

        var results= await userManager.CreateAsync(u, u.Password);
        if(!results.Succeeded){
            foreach(var e in results.Errors){
                ModelState.AddModelError(e.Code, e.Description);
            }
            return BadRequest(ModelState);
        }

        foreach (var r in u.Roles)
        {
            await userManager.AddToRoleAsync(u, r);
        }
        
        await signInManager.SignInAsync(u, false);

        return Accepted("registered user");
        
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("model not valid");
        }

        
   
        try
        {   
            //var user = new User { UserName = userModel.UserName, Password = userModel.Password };
            //var result = await userManager.CreateAsync(user, userModel.Password);
            User u=await userManager.FindByNameAsync(userModel.UserName);
        
            //Console.WriteLine(await userManager.CheckPasswordAsync(u, userModel.Password));

            //var isCorrect = await userManager.CheckPasswordAsync(user, userModel.Password);


            if(! await authManager.ValidateUser(userModel, u)){
                return Unauthorized();
            }

            
            return Accepted(new {Token=await authManager.CreateToken(u)});
            
        }
        catch (Exception ex)
        {
            
            return Problem (ex.HelpLink, ex.StackTrace, statusCode: 500);
        }
    

    }
    
}