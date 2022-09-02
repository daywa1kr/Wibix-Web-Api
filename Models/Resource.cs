using System.ComponentModel.DataAnnotations.Schema;

namespace wibix_api.Models;

public class CreateResource{
    public string? Title{get; set;}
    public string? CourseName{get; set;}
    public string? Description{get; set;}
    public string? School{get; set;}
    [NotMapped]
    public IFormFile? File{get; set;}
}

public class Resource :CreateResource{
    public int Id{get; set;}
    public DateTime? Date{get; set;}
    public string? FileName{get; set;}
    public Course Course{get; set;}=null!;
    public int Rating{get; set;}
    public int CourseId{get; set;}
    public int SchoolId{get; set;}
    public IList<Resource> Similar{get;set;}=null!;
}