using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : Controller{
    private AppDbContext ctx{get; set;}
    public ForumController(AppDbContext _ctx){
        ctx=_ctx; 
    }

    [HttpGet("Upvoted")]
    public IActionResult GetUpvoted(){
        List<Post> posts=ctx.Posts.OrderBy(p=>p.Rating).ToList();
        posts.Reverse();
        return Ok(posts);
    }

    [HttpGet("Recent")]
    public IActionResult GetRecent(){
        List<Post> posts=ctx.Posts.OrderBy(p=>p.Date).ToList();
        posts.Reverse();
        return Ok(posts);
    }

    [HttpGet("Hottest")]
    public IActionResult GetHottest(){
        List<Post> posts=ctx.Posts.OrderBy(p=>p.AnswerCount).ToList();
        posts.Reverse();
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPost(int id){
        var x=ctx.Posts.Find(id);
        List<Answer> answers=ctx.Answers.Where(a=>a.PostId==id).OrderBy(a=>a.Rating).ToList();
        answers.Reverse();
            if(x!=null)
                x.Answers=answers; 
        return Ok(x);
    }

    [Authorize]
    [HttpPost("AddPost")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddPost([FromBody]CreatePost post){
        Post p=new Post();
        p.Heading=post.Heading;
        p.Body=post.Body;
        p.Rating=0;
        p.Date=DateTime.Now;
        p.Answers=new List<Answer>();
        p.AnswerCount=0;
        ctx.Posts.Add(p);
        ctx.SaveChanges();
        return Ok();
    }

    [Authorize]
    [HttpPost("AddAnswer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddAnswer(CreateAnswer ans){
        Answer a=new Answer();
        a.Body=ans.Body;
        a.PostId=ans.PostId;
        Post p=ctx.Posts.Find(ans.PostId);
        p.AnswerCount+=1;
        a.Post=p;
        a.Date=DateTime.Now;
        a.Rating=0;
        ctx.Answers.Add(a);
        ctx.SaveChanges();
        return Ok();
    }

    [Authorize]
    [HttpPost("UpvotePost/{id:int}")]
    public IActionResult Upvote(int id) {
        var x=ctx.Posts.Find(id);
        x.Rating+=1;
        ctx.SaveChanges();
        return Ok();
    }

    [Authorize]
    [HttpPost("DownvotePost/{id:int}")]
    public IActionResult Downvote(int id) {
        var x=ctx.Posts.Find(id);
        x.Rating-=1;
        ctx.SaveChanges();
        return Ok();
    }

    // [HttpPost("EditPost")]
    // public IActionResult Edit(Post s){
    //     if (!ModelState.IsValid){
    //         return StatusCode(500);
    //     }
    //     ctx.Posts.Update(s);
    //     ctx.SaveChanges();
    //     return Ok(s);
    // }

    // public IActionResult UpvoteAnswer(int id) {
    //     var x=ctx.Answers.Find(id);
    //     if(x==null)
    //         return RedirectToAction("ViewPost");
    //     x.Rating+=1;
    //     ctx.SaveChanges();
    //     return RedirectToAction("ViewPost", ctx.Posts.Find(x.PostId));
    // }

    // public IActionResult DownvoteAnswer(int id) {
    //     var x=ctx.Answers.Find(id);
    //     if(x==null)
    //         return RedirectToAction("ViewPost");
    //     x.Rating-=1;
    //     ctx.SaveChanges();
    //     return RedirectToAction("ViewPost", ctx.Posts.Find(x.PostId));
    // }

}