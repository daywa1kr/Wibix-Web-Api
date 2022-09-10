using System.ComponentModel.DataAnnotations.Schema;

namespace wibix_api.Models;

public class CreatePost{
    public string? Heading{get; set;}
    public string? Body{get; set;}
}

public class Post : CreatePost{
    public int Id{get; set;}
    public DateTime Date{get; set;} 
    public int Rating{get; set;}
    public int AnswerCount{get; set;}
    public IEnumerable<Answer> Answers{get; set;}=null!;
    public string UserId{get; set;}=null!;
    [NotMapped]
    public VisibleInfo User{get; set;}=null!;
}