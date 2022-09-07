using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wibix_api.Models;

namespace wibix_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourceController : Controller
{
    public static IWebHostEnvironment env{get; set;}=null!;
    public static AppDbContext ctx{get;set;}=null!;
    private readonly UserManager<User> userManager=null!;
    public ResourceController(IWebHostEnvironment _env, AppDbContext _ctx, UserManager<User> _userManager)
    {
        env=_env;
        ctx=_ctx;
        userManager=_userManager;
    }

    [HttpGet("Schools")]
    public IActionResult GetSchools(){
        return Ok(ctx.Schools.OrderBy(s=>s.Name).ToList());
    }

    [HttpGet("AllCourses")]
    public IActionResult GetAllCourses(){
        return Ok(ctx.Courses.ToList());
    }

    [HttpGet("AllInSchool/{id:int}")]
    public IActionResult GetCoursesOfSchool(int id){ //get all cources in school and sort by name | for ALL
        return Ok(ctx.Courses.Where(c=>c.SchoolId==id).OrderBy(c=>c.CourseName).ToList());
    }

    [HttpGet("Courses/{order}/{id:int}")]  //find course by id |For navigation
    public IActionResult GetCourse(int id, string order){
        var x=ctx.Courses.Find(id);
        List<Resource> resources= (order.Equals("name"))? ctx.Resources.Where(a=>a.CourseId==id).OrderBy(a=>a.CourseName).ToList(): ctx.Resources.Where(a=>a.CourseId==id).OrderBy(a=>a.Rating).ToList();
        School s=ctx.Schools.Find(x.SchoolId);
        if(!order.Equals("name")){
            resources.Reverse();
        }
        if(x!=null){
            x.Resources=resources; 
            x.School=s;
        }
        return Ok(x);
    }

    [HttpGet("Schools/{order}/{id:int}")] //get school with id and orders courses by 'order'
    public IActionResult GetSchool(int id, string order){
        var x=ctx.Schools.Find(id);
        List<Course> courses= (order.Equals("name"))?ctx.Courses.Where(a=>a.SchoolId==id).OrderBy(a=>a.CourseName).ToList():ctx.Courses.Where(a=>a.SchoolId==id).OrderBy(a=>a.NumberOfRes).ToList();
        if(!order.Equals("name"))
            courses.Reverse();
        if(x!=null)
            x.Courses=courses; 
        return Ok(x);
    }

    [HttpGet("{id:int}")] //get res with id 
    public async Task<IActionResult> GetResource(int id){
        var x=ctx.Resources.Find(id);
        
        if(x!=null){
            List<Resource> similar=ctx.Resources.Where(c=>(c.CourseId==x.CourseId && c.Id!=x.Id)).OrderBy(a=>a.Rating).ToList();
            x.Similar=similar;
        }

        if(x!=null){
            User u=await userManager.FindByIdAsync(x.UserId);

            VisibleInfo user=new VisibleInfo();
            
            user.Id=u.Id;
            user.DisplayName=u.DisplayName;
            user.UserName=u.UserName;
            user.Email=u.Email;
            user.Rating=u.Rating;
            user.ImageSrc=u.ImageSrc;
            user.Bio=u.Bio;
            
            x.User=user;
        }
        return Ok(x);
    }

    private int FindSchoolIdByName(string name){
       var query=ctx.Schools.Where(s=>s.Name==name).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    [HttpGet("SID/{name}")] //gets school id by name for search
    public IActionResult GetSchoolIdByName(string name){
       var query=ctx.Schools.Where(s=>s.Name.ToLower()==name.ToLower()).ToList();

        if(query==null || query.Count==0)
           return BadRequest(-1);
        return Ok(query[0].Id);
    }

    [HttpGet("SSD/{name}/{id:int}")] //gets course id by name and school id for search
    public IActionResult GetCourseIdByName(string name, int id){
       var query=ctx.Courses.Where(s=>(s.CourseName.ToLower()==name.ToLower() && s.SchoolId==id)).ToList();

        if(query==null || query.Count==0)
           return BadRequest(-1);
        return Ok(query[0].Id);
    }

    [HttpGet("SRI/{title}/{id:int}")] //gets course id by name and school id for search
    public IActionResult GetResourceIdByName(string title, int id){
       var query=ctx.Resources.Where(s=>(s.Title.ToLower()==title.ToLower() && s.CourseId==id)).ToList();

        if(query==null || query.Count==0)
           return BadRequest(-1);
        return Ok(query[0].Id);
    }

    [HttpGet("GetResourcesByUserId/{id}")]
    public IActionResult GetResourcesByUserId(string id){
        var posts=ctx.Resources.Where(p=>p.UserId.Equals(id));
        return Ok(posts);
    }


    [HttpPost("Like/{id:int}")]
    public IActionResult LikeResource(int id){
        var x=ctx.Resources.Find(id);
        x.Rating+=1;
        ctx.Resources.Update(x);
        ctx.SaveChanges();
        return Ok();
    }

    [HttpPost("Dislike/{id:int}")]
    public IActionResult DislikeResource(int id){
        var x=ctx.Resources.Find(id);
        x.Rating-=1;
        ctx.Resources.Update(x);
        ctx.SaveChanges();
        return Ok();
    }

    private int FindCourseIdByName(string name){
       var query=ctx.Courses.Where(s=>s.CourseName==name).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    [Authorize]
    [HttpPost("Upload/{id}")]
    public async Task<IActionResult> Upload([FromForm]CreateResource res, string id)
    {
        if(res.File!=null)
        {

            User u= await userManager.FindByIdAsync(id);
            VisibleInfo user=new VisibleInfo{
                Id=u.Id,
                DisplayName=u.DisplayName,
                UserName=u.UserName,
                Email=u.Email,
                Rating=u.Rating,
                ImageSrc=u.ImageSrc,
                Bio=u.Bio
            };

            string fileName=new String(Path.GetFileNameWithoutExtension(res.File.FileName).Take(10).ToArray()).Replace(' ', '-');
            fileName=fileName+DateTime.Now.ToString("yymmssfff")+Path.GetExtension(res.File.FileName);
            string serverFolder=Path.Combine(env.WebRootPath, "Uploads/", fileName);

            res.File.CopyTo(new FileStream(serverFolder, FileMode.Create));

            int schoolId=FindSchoolIdByName(res.School);

            //school does not exist then course wont exist either
            if(schoolId==-1){

                School s=new School(){
                    Name=res.School,
                    Courses=new List<Course>(),
                    NumberOfRes=1
                };

                ctx.Schools.Attach(s);
                ctx.SaveChanges();

                Course c=new Course();
                c.CourseName=res.CourseName;                
                c.School=s;
                c.SchoolId=s.Id;
                c.Resources=new List<Resource>();
                c.NumberOfRes=1;

                Resource r=new Resource(){
                    CourseName=res.CourseName,
                    Title=res.Title,
                    School=res.School,
                    File=res.File,
                    Description=res.Description,
                    FileName=fileName,
                    Rating=0,
                    CourseId=0,
                    SchoolId=s.Id,
                    UserId=id
                };
                r.Date=DateTime.Now;

                ctx.Resources.Add(r);
                c.Resources.Add(r);
                s.Courses.Add(c);
                ctx.Schools.Attach(s); //come to this this might cause a bug of there being 2 identical items
                ctx.SaveChanges();
                return Ok("created new school with new course");
            }
            //school exists
            else{
                int courseId=FindCourseIdByName(res.CourseName);
                //course doesnt exist
                if(courseId==-1){
                    Resource r=new Resource(){
                        CourseName=res.CourseName,
                        Title=res.Title,
                        School=res.School,
                        File=res.File,
                        Description=res.Description,
                        FileName=fileName,
                        Rating=0,
                        CourseId=0,
                        Date=DateTime.Now,
                        UserId=id
                    };
                    

                    Course c=new Course();
                    c.CourseName=res.CourseName;
                    c.SchoolId=schoolId;
                    c.School=ctx.Schools.Find(schoolId);
                    c.Resources=new List<Resource>();
                    c.NumberOfRes=1;
                    c.Resources.Add(r);

                    School s=ctx.Schools.Find(schoolId);
                    s.NumberOfRes++;
                    r.SchoolId=s.Id;

                    ctx.Resources.Add(r);
                    ctx.Courses.Add(c);
                    ctx.Schools.Update(s);
                    ctx.SaveChanges();
                    return Ok("created course");
                }
                //course exists
                else{
                    Resource r=new Resource(){
                        CourseName=res.CourseName,
                        Title=res.Title,
                        School=res.School,
                        File=res.File,
                        Description=res.Description,
                        FileName=fileName,
                        CourseId=courseId,
                        Rating=0,
                        Date=DateTime.Now,
                        UserId=id
                    };

                    ctx.Resources.Add(r);
                    Course c=ctx.Courses.Find(courseId);
                    School s=ctx.Schools.Find(schoolId);
                    c.NumberOfRes++;
                    r.SchoolId=s.Id;
                    c.Resources.Add(r);
                    s.NumberOfRes++;
                    ctx.Courses.Update(c);
                    ctx.Schools.Update(s);
                    ctx.SaveChanges();

                    return Ok("added resource to existing course");

                }
            }

            //string folder="Uploads/";
            //int id=FindCourseIdByName(res.CourseName);
            
        }
        else{
            return BadRequest("file null");
        }
    }


    
}