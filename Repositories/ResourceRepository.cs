using Microsoft.AspNetCore.Identity;
using wibix_api.Models;

namespace wibix_api.Repositories;

public class ResourceRepository: IResourceRepository{
    public static IWebHostEnvironment _env{get; set;}=null!;
    public readonly AppDbContext _ctx=null!;
    private readonly UserManager<User> _userManager=null!;
    public ResourceRepository(IWebHostEnvironment env, AppDbContext ctx, UserManager<User> userManager)
    {
        _env=env;
        _ctx=ctx;
        _userManager=userManager;
    }

    public IEnumerable<School> GetSchools()
    {
        return _ctx.Schools.OrderBy(s=>s.Name).ToList();
    }

    public IEnumerable<Course> GetCourses()
    {
        return _ctx.Courses.ToList();
    }

    public IEnumerable<Course> GetCoursesBySchoolId(int id)
    {
        return _ctx.Courses.Where(c=>c.SchoolId==id).OrderBy(c=>c.CourseName).ToList();
    }

    public async Task<Course> GetCourse(int id, string order)
    {
        var course = await _ctx.Courses.FindAsync(id);

        var resources = order.Equals("name")? 
                        _ctx.Resources.Where(a=>a.CourseId==id).OrderBy(a=>a.CourseName).ToList() : 
                        _ctx.Resources.Where(a=>a.CourseId==id).OrderBy(a=>a.Rating).ToList();

        if(!order.Equals("name")){
            resources.Reverse();
        }
        
        var school=_ctx.Schools.Find(course?.SchoolId);
        if(course!=null){
            course.Resources=resources;
            course.School=school;
            return course;
        }
        return null!;
    }

    public async Task<School> GetSchool(int id, string order)
    {
        var school= await _ctx.Schools.FindAsync(id);

        var courses = (order.Equals("name")) ?
                    _ctx.Courses.Where(a=>a.SchoolId==id).OrderBy(a=>a.CourseName).ToList():
                    _ctx.Courses.Where(a=>a.SchoolId==id).OrderBy(a=>a.NumberOfRes).ToList();

        if(!order.Equals("name"))
            courses.Reverse();

        if(school!=null)
        {
            school.Courses=courses; 
            return school;
        }
        return null!;
    }

    public async Task<Resource> GetResource(int id)
    {
        var res=await _ctx.Resources.FindAsync(id);

        if(res!=null)
        {
            var similar=_ctx.Resources.Where(c=>(c.CourseId==res.CourseId && c.Id!=res.Id)).OrderBy(a=>a.Rating).ToList();
            res.Similar=similar;

            User u=await _userManager.FindByIdAsync(res.UserId);

            VisibleInfo user=new VisibleInfo{
                Id=u.Id,
                DisplayName=u.DisplayName,
                UserName=u.UserName,
                Email=u.Email,
                Rating=u.Rating,
                ImageSrc=u.ImageSrc,
                Bio=u.Bio
            };
            
            res.User=user;
            return res;
        }

        return null!;

    }

    public int GetSchoolIdByName(string name)
    {
        var query=_ctx.Schools.Where(s=>s.Name.ToLower()==name.ToLower()).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    public int GetCourseIdByName(string name, int schoolId)
    {
        var query=_ctx.Courses.Where(s=>(s.CourseName.ToLower().Equals(name.ToLower()) && s.SchoolId==schoolId)).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    public int GetResourceIdByName(string name, int courseId)
    {
        var query=_ctx.Resources.Where(s=>(s.Title.ToLower()==name.ToLower() && s.CourseId==courseId)).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    public IEnumerable<Resource> GetResourcesByUserId(string id)
    {
        return _ctx.Resources.Where(r=>r.UserId.Equals(id));
    }

    public async Task Upvote(int id)
    {
        var res=await _ctx.Resources.FindAsync(id);
        if(res!=null)
            res.Rating+=1;
        await _ctx.SaveChangesAsync();
    }

    public async Task Downvote(int id)
    {
        var res=await _ctx.Resources.FindAsync(id);
        if(res!=null)
            res.Rating-=1;
        await _ctx.SaveChangesAsync();
    }

    public async Task<string> Upload(CreateResource model, string userId)
    {
       
        User u= await _userManager.FindByIdAsync(userId);
        VisibleInfo user=new VisibleInfo{
            Id=u.Id,
            DisplayName=u.DisplayName,
            UserName=u.UserName,
            Email=u.Email,
            Rating=u.Rating,
            ImageSrc=u.ImageSrc,
            Bio=u.Bio
        };

            
        u.Rating+=10;
        await _userManager.UpdateAsync(u);

        string fileName=new String(Path.GetFileNameWithoutExtension(model.File.FileName).Take(10).ToArray()).Replace(' ', '-');
        fileName=fileName+DateTime.Now.ToString("yymmssfff")+Path.GetExtension(model.File.FileName);
        
        string serverFolder=Path.Combine(_env.WebRootPath, "Uploads/", fileName);

        model.File.CopyTo(new FileStream(serverFolder, FileMode.Create));

        int schoolId=FindSchoolIdByName(model.School);

        //school does not exist then course wont exist either
        if(schoolId==-1){
            School s=new School(){
                Name=model.School,
                Courses=new List<Course>(),
                NumberOfRes=1
            };

            _ctx.Schools.Attach(s);
            await _ctx.SaveChangesAsync();

            Course c=new Course{
                CourseName=model.CourseName,   
                School=s,
                SchoolId=s.Id,
                Resources=new List<Resource>(),
                NumberOfRes=1
            };
                            
            Resource r=new Resource{
                CourseName=model.CourseName,
                Title=model.Title,
                School=model.School,
                File=model.File,
                Description=model.Description,
                FileName=fileName,
                Rating=0,
                CourseId=0,
                SchoolId=s.Id,
                UserId=userId,
                Date=DateTime.Now
            };
                
            await _ctx.Resources.AddAsync(r);
            
            c.Resources.Add(r);
            s.Courses.Add(c);

            _ctx.Schools.Attach(s);
            await _ctx.SaveChangesAsync();
        }
        //school exists
        else{
            int courseId=FindCourseIdByName(model.CourseName);
            //course doesnt exist
            if(courseId==-1){
                var s= await _ctx.Schools.FindAsync(schoolId);

                Resource r=new Resource{
                    CourseName=model.CourseName,
                    Title=model.Title,
                    School=model.School,
                    File=model.File,
                    Description=model.Description,
                    FileName=fileName,
                    Rating=0,
                    CourseId=0,
                    Date=DateTime.Now,
                    UserId=userId
                };
                    
                Course c=new Course(){
                    CourseName=model.CourseName,
                    SchoolId=schoolId,
                    School=s,
                    Resources=new List<Resource>(),
                    NumberOfRes=1,
                };
                c.Resources.Add(r);
                    
                if(s!=null){
                    s.NumberOfRes+=1;
                    r.SchoolId=s.Id;                
                    _ctx.Schools.Update(s);
                }

                await _ctx.Resources.AddAsync(r);
                await _ctx.Courses.AddAsync(c);
                await _ctx.SaveChangesAsync();
            }
                //course exists
            else{
                Resource r=new Resource{
                    CourseName=model.CourseName,
                    Title=model.Title,
                    School=model.School,
                    File=model.File,
                    Description=model.Description,
                    FileName=fileName,
                    CourseId=courseId,
                    Rating=0,
                    Date=DateTime.Now,
                    UserId=userId
                };

                await _ctx.Resources.AddAsync(r);
                
                Course? c= await _ctx.Courses.FindAsync(courseId);
                School? s=await _ctx.Schools.FindAsync(schoolId);

                if(c!=null)
                {
                    c.NumberOfRes++;
                    c.Resources.Add(r);
                    _ctx.Courses.Update(c);
                }
                
                if(s!=null){
                    r.SchoolId=s.Id;
                    s.NumberOfRes++;
                    _ctx.Schools.Update(s);
                }
                _ctx.SaveChanges();

            }
        }
        return serverFolder;
    }

    private int FindSchoolIdByName(string name){
       var query=_ctx.Schools.Where(s=>s.Name==name).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }

    private int FindCourseIdByName(string name){
       var query=_ctx.Courses.Where(s=>s.CourseName==name).ToList();

        if(query==null || query.Count==0)
           return -1;
        return query[0].Id;
    }
}