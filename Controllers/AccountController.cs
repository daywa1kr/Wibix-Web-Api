using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
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
    public static IWebHostEnvironment env{get; set;}=null!;
    public AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager, IAuthManager _authManager, IWebHostEnvironment _env){
        userManager=_userManager;
        signInManager=_signInManager;
        authManager=_authManager;
        env=_env;
    }

    [HttpGet("Users")]
    public IActionResult GetUsers(){

        return Ok(userManager.Users.ToList());
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetUser(){
        string username=User.Claims.First(c=>c.Type=="UserName").Value;
        var user= await userManager.FindByNameAsync(username);
        return Ok(user);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegister user){
        if(!ModelState.IsValid)
            return BadRequest();
        
        User u=new User();
        u.Bio="";
        u.UserName=user.UserName;
        u.Email=user.Email;
        u.Roles=new List<string>(){"User"};

        var results= await userManager.CreateAsync(u, user.Password);
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

        return Accepted(u);
        
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
            User u=await userManager.FindByNameAsync(userModel.UserName);

            if(! await authManager.ValidateUser(userModel, u)){
                return Unauthorized();
            }

            return Accepted(new {Token=await authManager.CreateToken(u),
            User=u});
            
        }
        catch (Exception ex)
        {  
            return Problem (ex.HelpLink, ex.StackTrace, statusCode: 500);
        }
    }

    [Authorize]
    [HttpPost("UploadPfp/{id}")]
    public async Task<IActionResult> Upload(IFormFile img, string id)
    {
        if(img!=null)
        {
            string fileName=new String(Path.GetFileNameWithoutExtension(img.FileName).Take(10).ToArray()).Replace(' ', '-');
            fileName=fileName+DateTime.Now.ToString("yymmssfff")+Path.GetExtension(img.FileName);
            string serverFolder=Path.Combine(env.WebRootPath, "Pfps/", fileName);

            img.CopyTo(new FileStream(serverFolder, FileMode.Create));

            var user=await userManager.FindByIdAsync(id);

            user.ImageSrc=fileName;

            await userManager.UpdateAsync(user);

            return Ok("added pfp");

        }
        else{
            return BadRequest("file null");
        }
    }
}