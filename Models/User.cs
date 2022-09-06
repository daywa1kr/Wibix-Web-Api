using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace wibix_api.Models;

public class UserLogin{
    public string UserName{get; set;}=null!;
    public string Password{get; set;}=null!;
}

public class UserRegister:UserLogin{
    [Required]
    [DataType(DataType.EmailAddress)]
   public string? Email {get; set;}
}

public class UpdateProfile{
    public string? Id{get; set;}

    public string? Email{get; set;}

    public string? Bio{get; set;}
    [NotMapped]
    public IFormFile? File{get; set;}
}

public class User : IdentityUser{

    // public string UserName{get; set;}=null!;
    //public string Password{get; set;}=null!;

    // public string? Email {get; set;}

   [NotMapped]
   public IFormFile ProfilePicture{get; set;}=null!;

   public string Bio{get; set;}=null!;

   public string? ImageSrc{get; set;}

    [NotMapped]
   public IList<string> Roles{get; set;}=null!;

}