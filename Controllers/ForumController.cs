using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : Controller{
    private AppDbContext ctx{get; set;}
   private readonly UserManager<User> userManager=null!;
    public ForumController(AppDbContext _ctx, UserManager<User> _userManager){
        ctx=_ctx; 
        userManager=_userManager;
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
    public async Task<IActionResult> GetPost(int id){
        var x=ctx.Posts.Find(id);
        List<Answer> answers=ctx.Answers.Where(a=>a.PostId==id).OrderBy(a=>a.Rating).ToList();
        answers.Reverse();

        if(x!=null){
            User u=await userManager.FindByIdAsync(x.UserId);

            VisibleInfo user=new VisibleInfo{
                Id=u.Id,
                DisplayName=u.DisplayName,
                UserName=u.UserName,
                Email=u.Email,
                Rating=u.Rating,
                ImageSrc=u.ImageSrc,
                Bio=u.Bio
            };
            x.Answers=answers;
            x.User=user;
        }
        
        return Ok(x);
    }

    [Authorize]
    [HttpPost("AddPost/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPost([FromBody]CreatePost post, string id){
        Post p=new Post();
        p.Heading=post.Heading;
        p.Body=post.Body;
        p.Rating=0;
        p.Date=DateTime.Now;
        p.Answers=new List<Answer>();
        p.AnswerCount=0;
        p.UserId=id;
        
        User u=await userManager.FindByIdAsync(id);
        VisibleInfo user=new VisibleInfo{
            Id=u.Id,
            DisplayName=u.DisplayName,
            UserName=u.UserName,
            Email=u.Email,
            Rating=u.Rating,
            ImageSrc=u.ImageSrc,
            Bio=u.Bio
        };

        p.User=user;

        ctx.Posts.Add(p);
        ctx.SaveChanges();
        return Ok("post added");
    }

    [Authorize]
    [HttpPost("AddAnswer/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAnswer(CreateAnswer ans, string id){
        Answer a=new Answer();
        a.Body=ans.Body;
        a.PostId=ans.PostId;
        Post p=ctx.Posts.Find(ans.PostId);
        p.AnswerCount+=1;
        a.Post=p;
        a.Date=DateTime.Now;
        a.Rating=0;
        a.UserId=id;

        User u=await userManager.FindByIdAsync(id);
        VisibleInfo user=new VisibleInfo{
            Id=u.Id,
            DisplayName=u.DisplayName,
            UserName=u.UserName,
            Email=u.Email,
            Rating=u.Rating,
            ImageSrc=u.ImageSrc,
            Bio=u.Bio
        };

        a.User=user;
        ctx.Answers.Add(a);
        ctx.SaveChanges();
        return Ok("added answer");
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