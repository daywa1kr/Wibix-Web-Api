using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using wibix_api.Models;

namespace wibix_api.Services;

public class AuthManager:  IAuthManager{

    private readonly UserManager<User> userManager=null!;
    private readonly IConfiguration configuration=null!;

    //public User user{get; set;}

    public AuthManager(UserManager<User> _userManager, IConfiguration _configuration)
    {
        userManager=_userManager;
        configuration=_configuration;
        //user=_user;
    }

    public async Task<string> CreateToken(User user)
    {
        var signingCredentials=GetSigningCredentials();
        var claims= await GetClaims(user);
        var tokenOptions=GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims=new List<Claim>{
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var roles=await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var sectret=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt").GetSection("Key").Value));

        return new SigningCredentials(sectret, SecurityAlgorithms.HmacSha256);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSetting=configuration.GetSection("Jwt");
        var expiration=DateTime.Now.AddMinutes(Convert.ToDouble(jwtSetting.GetSection("Lifetime").Value));
        var token=new JwtSecurityToken(
            issuer: jwtSetting.GetSection("Issuer").Value,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        return token;
    }

    public async Task<bool> ValidateUser(UserLogin model, User user)
    {
        user=await userManager.FindByNameAsync(model.UserName);
        return (user!=null && await userManager.CheckPasswordAsync(user, model.Password));
    }
}