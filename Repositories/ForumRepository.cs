using Microsoft.AspNetCore.Identity;
using wibix_api.Models;

namespace wibix_api.Repositories;

public class ForumRepository : IForumRepository
{
    private readonly AppDbContext _ctx;
    private readonly UserManager<User> _userManager;

    public ForumRepository(AppDbContext ctx, UserManager<User> userManager)
    {
        _ctx=ctx;
        _userManager=userManager;
    }
    
    public async Task AddAnswer(CreateAnswer model, string userId)
    {
        if(model!=null){
            Post? p=await _ctx.Posts.FindAsync(model.PostId);
            User u=await _userManager.FindByIdAsync(userId);

            VisibleInfo user=new VisibleInfo{
                Id=u.Id,
                DisplayName=u.DisplayName,
                UserName=u.UserName,
                Email=u.Email,
                Rating=u.Rating,
                ImageSrc=u.ImageSrc,
                Bio=u.Bio
            };

            u.Rating+=5;
            await _userManager.UpdateAsync(u);

            Answer a=new Answer{
                Body=model.Body,
                PostId=model.PostId,
                Post=p,
                Date=DateTime.Now,
                Rating=0,
                UserId=userId,
                User=user
            };

            await _ctx.Answers.AddAsync(a);
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task AddPost(CreatePost model, string userId)
    {
        User u=await _userManager.FindByIdAsync(userId);
        VisibleInfo user=new VisibleInfo{
            Id=u.Id,
            DisplayName=u.DisplayName,
            UserName=u.UserName,
            Email=u.Email,
            Rating=u.Rating,
            ImageSrc=u.ImageSrc,
            Bio=u.Bio
        };
        Post p=new Post{
            Heading=model.Heading,
            Body=model.Body,
            Rating=0,
            Date=DateTime.Now,
            Answers=new List<Answer>(),
            AnswerCount=0,
            UserId=userId,
            User=user
        };

        u.Rating+=3;
        await _userManager.UpdateAsync(u);

        await _ctx.Posts.AddAsync(p);
        await _ctx.SaveChangesAsync();
    }

    public async Task Downvote(int id)
    {
        var p=await _ctx.Posts.FindAsync(id);
        if(p!=null)
            p.Rating-=1;
        await _ctx.SaveChangesAsync(); 
    }

    public async Task DownvoteAnswer(int id)
    {
        var a=await _ctx.Answers.FindAsync(id);
        if(a!=null)
            a.Rating-=1;
        await _ctx.SaveChangesAsync(); 
    }

    public IEnumerable<Answer> GetAnswers(int postId)
    {
        var answers=_ctx.Answers.Where(a=>a.PostId==postId).ToList();
        answers.Reverse();

        return answers;
    }
    public async Task<Post> GetPost(int id)
    {
        var p=await _ctx.Posts.FindAsync(id);

        var answers=GetAnswers(id);

        foreach (var a in answers)
        {
            User u=await _userManager.FindByIdAsync(a.UserId);
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
        }

        if(p!=null)
        {
            User u=await _userManager.FindByIdAsync(p.UserId);

            VisibleInfo user=new VisibleInfo{
                Id=u.Id,
                DisplayName=u.DisplayName,
                UserName=u.UserName,
                Email=u.Email,
                Rating=u.Rating,
                ImageSrc=u.ImageSrc,
                Bio=u.Bio
            };
            p.Answers=answers;
            p.User=user;
            return p;
        }

        throw new ArgumentNullException();
    }

    public IEnumerable<Post> GetPosts(string order)
    {
        List<Post> posts;
        if(order.Equals("upvoted"))
        {
            posts=_ctx.Posts.OrderBy(p=>p.Rating).ToList();
            posts.Reverse();
            return posts;
        }
        if(order.Equals("date"))
        {
            posts=_ctx.Posts.OrderBy(p=>p.Date).ToList();
            posts.Reverse();
            return posts;
        }
        if(order.Equals("answers"))
        {
            posts=_ctx.Posts.OrderBy(p=>p.AnswerCount).ToList();
            posts.Reverse();
            return posts;
        }
        throw new InvalidDataException();
        
    }

    public IEnumerable<Post> GetPostsByUserId(string id)
    {
        var posts=_ctx.Posts.Where(p=>p.UserId.Equals(id));
        return posts;
    }

    public async Task Upvote(int id)
    {
        var p=await _ctx.Posts.FindAsync(id);
        if(p!=null)
            p.Rating+=1;
        await _ctx.SaveChangesAsync(); 
    }

    public async Task UpvoteAnswer(int id)
    {
        var a=await _ctx.Answers.FindAsync(id);
        if(a!=null)
            a.Rating+=1;
        await _ctx.SaveChangesAsync(); 
    }
}