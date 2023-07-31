using System.Text;
using Shop.Data;
using Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Settings.Secret);

// Add services to the container.
builder.Services.AddResponseCompression(options =>
{
  options.Providers.Add<GzipCompressionProvider>();
  options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});
builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.AddAuthentication(configureOptions: x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configureOptions: x =>
{
  x.RequireHttpsMetadata = false;
  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});


//builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

builder.Services.AddDbContext<DataContext>(optionsAction: opt => opt.UseInMemoryDatabase(databaseName: "Database"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop APi", Version = "v1" });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(setupAction: c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
});

app.UseRouting();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
