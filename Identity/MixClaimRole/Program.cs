using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MixClaimRole.Contexts;
using MixClaimRole.Models;
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
        Title = "Sample of MixOfClaimAndRole",
        Version = "v1",
    });
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WriteEmail", policy =>
    {
        policy.RequireClaim("WriteEmail");
    });

    options.AddPolicy("ReadEmail", policy =>
    {
        policy.RequireClaim("ReadEmail");
    });
});

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
        config.DocumentTitle = "Sample of MixOfClaimAndRoleAuth";
    });
}
app.MapControllers();


app.Run();