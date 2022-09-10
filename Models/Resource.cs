using System.ComponentModel.DataAnnotations.Schema;

namespace wibix_api.Models;

public class CreateResource{
    public string Title{get; set;}=null!;
    public string CourseName{get; set;}=null!;
    public string? Description{get; set;}
    public string School{get; set;}=null!;
    [NotMapped]
    public IFormFile File{get; set;}=null!;

}

public class Resource :CreateResource{
    public int Id{get; set;}
    public DateTime? Date{get; set;}
    public string? FileName{get; set;}
    public Course Course{get; set;}=null!;
    public int Rating{get; set;}
    public int CourseId{get; set;}
    public int SchoolId{get; set;}
    public string UserId{get; set;}=null!;
    [NotMapped]
    public VisibleInfo User{get; set;}=null!;
    public IList<Resource> Similar{get;set;}=null!;
}