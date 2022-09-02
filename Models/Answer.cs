namespace wibix_api.Models;

public class CreateAnswer{
    public string? Body{get; set;}
    public int PostId{get; set;}
}
public class Answer : CreateAnswer{
    public int Id{get; set;}
    public DateTime Date{get; set;} 
    public int Rating{get; set;}
    public Post Post{get; set;}=null!;
    
}