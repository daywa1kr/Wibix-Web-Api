using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wibix_api.Models;

public class AppDbContext : IdentityDbContext<User>{

    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}

    public AppDbContext() : base(){}
    public DbSet<Post> Posts {get; set;}=null!;
    public DbSet<Answer> Answers {get; set;}=null!;
    public DbSet<School> Schools{get; set;}=null!;
    public DbSet<Course> Courses{get; set;}=null!;
    public DbSet<Resource> Resources{get; set;}=null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole{
                Name="User",
                NormalizedName="USER"
            },
            new IdentityRole{
                Name="Admin",
                NormalizedName="ADMIN"
            },
            new IdentityRole{
                Name="Mod",
                NormalizedName="MOD"
            }
        );

        modelBuilder.Entity<Course>().HasMany(p=>p.Resources).WithOne(p=>p.Course);
        modelBuilder.Entity<School>().HasMany(p=>p.Courses).WithOne(p=>p.School);
        modelBuilder.Entity<Post>().HasMany(p=>p.Answers).WithOne(p=>p.Post);

        // modelBuilder.Entity<Post>().HasData(
        //     new Post{
        //         Id=1,
        //         Heading="Welcome to Wibix Forum",
        //         Body="<p>This is our first thread.</p><p>We are happy to have you here.</p>",
        //         Date=DateTime.Now,
        //         Rating=0,
        //         AnswerCount=1
        //     }
        // );
        // modelBuilder.Entity<Answer>().HasData(
        //     new Answer{
        //         Id=1,
        //         Body="This is our first answer",
        //         Date=DateTime.Now,
        //         Rating=0,
        //         PostId=1
        //     }
        // );
    }

}