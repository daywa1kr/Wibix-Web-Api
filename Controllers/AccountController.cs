using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;
using wibix_api.Repositories;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : Controller{
    private IAccountRepository _repo{get;set;}
    public AccountController(IAccountRepository repo)
    {
        _repo=repo;
    }

    [HttpGet("Users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetUsers()
    {
        return Ok(_repo.GetUsers());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(string id)
    {
        return Ok(await _repo.GetUser(id));
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserRegister user)
    {
        if(!ModelState.IsValid)
            return BadRequest("Model state not valid");
    
        var results= await _repo.Register(user);

        if(!results.Succeeded)
        {
            foreach(var e in results.Errors)
            {
                ModelState.AddModelError(e.Code, e.Description);
            }
            return BadRequest(ModelState);
        }
        return Accepted(); 
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
        try
        {   
            return Accepted(await _repo.Login(model));
        }
        catch (Exception ex)
        {  
            return Problem (ex.HelpLink, ex.StackTrace, statusCode: 500);
        }
    }

    [Authorize]
    [HttpPost("UpdateProfile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromForm]UserUpdate model)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest("model state not valid");
        }
        try
        {
            await _repo.UpdateProfile(model);
            return Ok("profile updated");
        }
        catch (Exception ex)
        {
            return Problem (ex.HelpLink, ex.StackTrace, statusCode: 500);
        }
    }

    [Authorize]
    [HttpDelete("DeleteUser/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _repo.DeletProfile(id);
        return Ok("user deleted");
    }
}