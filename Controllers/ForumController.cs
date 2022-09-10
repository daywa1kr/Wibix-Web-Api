using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;
using wibix_api.Repositories;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : Controller
{
    private IForumRepository _repo{get; set;}
    public ForumController(IForumRepository repo)
    {
        _repo=repo;
    }

    [HttpGet("Upvoted")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetUpvoted()
    {
        return Ok(_repo.GetPosts("upvoted"));
    }

    [HttpGet("Recent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetRecent()
    {
        return Ok(_repo.GetPosts("date"));
    }

    [HttpGet("Hottest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetHottest()
    {
        return Ok(_repo.GetPosts("answers"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPost(int id){
        return Ok(await _repo.GetPost(id));
    }

    [HttpGet("GetPostsByUserId/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetPostsByUserId(string id){
        if(id=="")
            return BadRequest("empty string");
        return Ok(_repo.GetPostsByUserId(id));
    }

    [Authorize]
    [HttpPost("AddPost/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPost([FromBody]CreatePost post, string id){
        await _repo.AddPost(post, id);
        return Ok("post created");
    }

    [Authorize]
    [HttpPost("AddAnswer/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAnswer(CreateAnswer answer, string id){
        await _repo.AddAnswer(answer, id);
        return Ok(StatusCodes.Status201Created);
    }

    [HttpPost("UpvotePost/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upvote(int id) {
        await _repo.Upvote(id);
        return Ok();
    }

    [HttpPost("DownvotePost/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Downvote(int id) {
        await _repo.Downvote(id);
        return Ok();
    }

    [HttpPost("UpvoteAnswer/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpvoteAnswer(int id) {
        await _repo.UpvoteAnswer(id);
        return Ok();
    }

    [HttpPost("DownvoteAnswer/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownvoteAnswer(int id) {
        await _repo.DownvoteAnswer(id);
        return Ok();
    }
}