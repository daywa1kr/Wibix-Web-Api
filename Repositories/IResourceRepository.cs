using Microsoft.AspNetCore.Identity;
using wibix_api.Models;

namespace wibix_api.Repositories;

public interface IResourceRepository
{
    /// <summary>
    /// Gets all schools in alphabetical order
    /// </summary>
    IEnumerable<School> GetSchools();

    /// <summary>
    /// Gets all courses in the database
    /// </summary>
    IEnumerable<Course> GetCourses();

    /// <summary>
    /// Gets courses of the school with specified
    /// <paramref name="id">id</paramref> ordered alphabetically
    /// </summary>
    IEnumerable<Course> GetCoursesBySchoolId(int id);

    /// <summary>
    /// Gets the course with specified
    /// <paramref name="id">id</paramref> and sorts it's resouces by <paramref name="order">order</paramref>
    /// </summary>
    Task<Course> GetCourse(int id, string order);

    /// <summary>
    /// Gets the school with specified
    /// <paramref name="id">id</paramref> and sorts it's courses by <paramref name="order">order</paramref>
    /// </summary>
    Task<School> GetSchool(int id, string order);

    /// <summary>
    /// Gets the resource with specified
    /// <paramref name="id">id</paramref>
    /// </summary>
    Task<Resource> GetResource(int id);

    /// <summary>
    /// Gets the id of the school with specified
    /// <paramref name="name">name</paramref>
    /// </summary>
    int GetSchoolIdByName(string name);

    /// <summary>
    /// Gets the id of the course with specified
    /// <paramref name="name">name</paramref>
    /// </summary>
    int GetCourseIdByName(string name, int schoolId);

    /// <summary>
    /// Gets the id of the resource with specified
    /// <paramref name="name">name</paramref>
    /// </summary>
    int GetResourceIdByName(string name, int courseId);

    /// <summary>
    /// Gets the resources posted by the user specified
    /// <paramref name="id">id</paramref>
    /// </summary>
    IEnumerable<Resource> GetResourcesByUserId(string id);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Resource"/> with specified
    /// <paramref name="id">id</paramref> and increments its Rating
    /// </summary>
    Task Upvote(int id);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Resource"/> with specified
    /// <paramref name="id">id</paramref> and decrements its Rating
    /// </summary>
    Task Downvote(int id);

    /// <summary>
    /// Creates a <see cref="wibix_api.Models.Resource"/> object from <paramref name="model"></paramref>, saves <see cref="Microsoft.AspNetCore.Http.IFormFile"/> of the model in a local directory, and adds it to the database with the specified 
    /// <paramref name="userId">userId</paramref>
    /// </summary>
    Task<string> Upload(CreateResource model, string userId);

}