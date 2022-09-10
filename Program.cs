using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.FileProviders;
using wibix_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using wibix_api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using wibix_api.Repositories;

var builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();

builder.Services.AddMvc().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Description=@"JWT Authorization header using the Bearer Scheme.
        Enter 'Bearer' [space] and then your token in the text input below.
        Example: 'Bearer 1234abcd'",
        Name="Authorization",
        In=ParameterLocation.Header,
        Type=SecuritySchemeType.ApiKey,
        Scheme="Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
        {
            new OpenApiSecurityScheme{
                Reference=new OpenApiReference(){
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="Oauth2",
                Name="Bearer",
                In=ParameterLocation.Header

            },

            new List<string>()
        }
    });
    
});

var provider=builder.Services.BuildServiceProvider();
var config=provider.GetRequiredService<IConfiguration>();

builder.Services.AddCors(options=>{
    var frontendUrl=config.GetValue<string>("frontend_url");
    options.AddDefaultPolicy(builder=>
    {
        builder.WithOrigins(frontendUrl).AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.AddDbContext<AppDbContext>(x=>{
    x.UseSqlite(builder.Configuration.GetConnectionString("default"));
});

builder.Services.AddIdentity<User, IdentityRole>(q=>q.User.RequireUniqueEmail=true).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options=>{
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>{
    options.TokenValidationParameters=new TokenValidationParameters{
        ValidateIssuer=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer=builder.Configuration.GetSection("Jwt").GetSection("Issuer").Value,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt").GetSection("Key").Value)),
        ValidAudience=builder.Configuration.GetSection("Jwt").GetSection("Audience").Value
    };
});

//builder.AddHostedService<User>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "Uploads")),
    RequestPath = new PathString("/Uploads")
});

app.MapControllers();

app.Run();


