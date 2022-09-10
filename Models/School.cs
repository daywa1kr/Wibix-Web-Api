using System.ComponentModel.DataAnnotations;

namespace wibix_api.Models;

public class School{
    public int Id{get; set;}
    [Required]
    public string Name{get; set;}=null!;
    public IList<Course> Courses{get; set;}=null!;
    public int NumberOfRes{get; set;}
}

public class Course{
    public int Id{get; set;}
    public string CourseName{get; set;}=null!;
    public int SchoolId{get; set;}
    public School? School{get; set;}=null!;
    public IList<Resource> Resources{get; set;}=null!;
    public int NumberOfRes{get; set;}
}