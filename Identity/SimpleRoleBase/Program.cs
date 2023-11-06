using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleRoleBase.Contexts;
using SimpleRoleBase.Models;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddControllers(options =>
{
    options.AddDefaultResultConvention();
});
builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("MainConnection"));
});

// config identity
builder.Services.AddIdentity<User, IdentityRole>(config =>
{
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireLowercase = true;
    config.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(config =>
{
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample of SimpleRoleBase",
        Version = "v1",
    });
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes("This is my supper secret key for jwt");
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "issuer",
        ValidAudience = "issuer",
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});


//await IdentityDataSeed(builder.Services.BuildServiceProvider());

//
var app = builder.Build();
//

app.UseAuthentication();
app.UseAuthorization();
// request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.DocumentTitle = "Sample of SimpleRoleBaseAuth";
    });
}
app.MapControllers();


app.Run();

async Task IdentityDataSeed(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    var roleExists = await roleManager.Roles.AnyAsync(x => x.Name == "Admin");
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
    }

    var user = await userManager.FindByNameAsync("admin");
    if (user == null)
    {
        User newUser = new User()
        {
            Id = "b74ddd14-6340-4840-95c2-db12554843e5",
            UserName = "Admin",
            Email = "admin@gmail.com",
            LockoutEnabled = false,
            PhoneNumber = "1234567890",
            FirstName = "Admin FirstName",
            LastName = "Admin LastName",
        };

        var createUserResult = await userManager.CreateAsync(newUser, "Admin@123");
        if (createUserResult.Succeeded)
        {
            var xxx = await userManager.AddToRoleAsync(newUser, "Admin");
        }
    }
}
