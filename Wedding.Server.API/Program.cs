using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Wedding.Server.API;
using Wedding.Server.API.Data;
using Wedding.Server.API.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SqlServerOptions>(options => options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddApplicationRepositories();
builder.Services.AddApplicationServices();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtOptions:Secret"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthentication(x =>
{	
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddCors();
builder.Services.AddControllers();
    // .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", 
        new OpenApiInfo 
        { 
            Title = "Leguto wedding API", 
            Version = "v1",
            Description = "API to serve the client application. See https://leguto.co",
            Contact = new OpenApiContact
            {
                Name = "Augusto Pereira",
                Email = "augustohtp8@gmail.com",
                Url = new Uri("https://github.com/augustohtpereira"),
            },
        }
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leguto wedding API");
   c.RoutePrefix = "docs";
});

app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
