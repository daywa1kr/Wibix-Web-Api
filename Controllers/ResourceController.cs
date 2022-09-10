using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;
using wibix_api.Repositories;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourceController : Controller
{
    private IResourceRepository _repo{get; set;}
    public ResourceController(IResourceRepository repo)
    {
        _repo=repo;
    }

    [HttpGet("Schools")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSchools()
    {
        return Ok(_repo.GetSchools());
    }

    [HttpGet("AllCourses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAllCourses()
    {
        return Ok(_repo.GetCourses());
    }

    [HttpGet("AllInSchool/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCoursesOfSchool(int id)
    {
        return Ok(_repo.GetCoursesBySchoolId(id));
    }

    [HttpGet("Courses/{order}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCourse(int id, string order)
    {
        return Ok(await _repo.GetCourse(id, order));
    }

    [HttpGet("Schools/{order}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSchool(int id, string order)
    {
        return Ok(_repo.GetSchool(id, order));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetResource(int id)
    {
        return Ok(await _repo.GetResource(id));
    }

    [HttpGet("SID/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSchoolIdByName(string name)
    {
        return Ok(_repo.GetSchoolIdByName(name));
    }

    [HttpGet("SSD/{name}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCourseIdByName(string name, int id)
    {
       return Ok(_repo.GetCourseIdByName(name, id));
    }

    [HttpGet("SRI/{title}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetResourceIdByName(string title, int id)
    {
        return Ok(_repo.GetResourceIdByName(title, id));
    }

    [HttpGet("GetResourcesByUserId/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetResourcesByUserId(string id)
    {
        return Ok(_repo.GetResourcesByUserId(id));
    }

    [HttpPost("UpvoteRes/{id:int}")]
    public async Task<IActionResult> UpvoteRes(int id) 
    {
        await _repo.Downvote(id);
        return Ok();
    }

    [HttpPost("DownvoteRes/{id:int}")]
    public async Task<IActionResult> DownvoteRes(int id) 
    {
        await _repo.Downvote(id);
        return Ok();
    }

    [Authorize]
    [HttpPost("Upload/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload([FromForm]CreateResource res, string id)
    {
        if(res.File!=null)
        {
            var uri=await _repo.Upload(res, id);
            return Created(uri, res);
        }
        else{
            return BadRequest("file null");
        }
    }
}