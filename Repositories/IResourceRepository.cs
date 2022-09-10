using Microsoft.AspNetCore.Identity;
using wibix_api.Models;

namespace wibix_api.Repositories;

public interface IResourceRepository
{
    IEnumerable<School> GetSchools();
    IEnumerable<Course> GetCourses();
    IEnumerable<Course> GetCoursesBySchoolId(int id);
    Task<Course> GetCourse(int id, string order);
    Task<School> GetSchool(int id, string order);
    Task<Resource> GetResource(int id);
    int GetSchoolIdByName(string name);
    int GetCourseIdByName(string name, int schoolId);
    int GetResourceIdByName(string name, int courseId);
    IEnumerable<Resource> GetResourcesByUserId(string id);
    Task Upvote(int id);
    Task Downvote(int id);
    Task<string> Upload(CreateResource model, string userId);

}