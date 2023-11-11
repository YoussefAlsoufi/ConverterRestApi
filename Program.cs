using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConverterRestApi.Data;
using System.Drawing.Text;
using ConverterRestApi;
using Microsoft.AspNetCore.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ConverterRestApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConverterRestApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConverterRestApiContext") ?? throw new InvalidOperationException("Connection string 'ConverterRestApiContext' not found.")));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
var dbContext = builder.Services.BuildServiceProvider().GetService<ConveterTools>();

var configuration = builder.Configuration;

// Configure services
var jwtSettings = configuration.GetSection("JWTSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddScoped<DataAccess>();
builder.Services.AddScoped<ConveterTools>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authKey = configuration.GetValue<string>("JWTSettings:SecretKey");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = false;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey))

    };
});

builder.Services.AddCors(p => p.AddPolicy("CorsPolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();  // here if we fill WithOrigins("http://localhost:4200") means only this domain ables to access my API.
                                                               // Or if we fill WithOrigins("http://localhost:4200","http://localhost:3000") for multi domains.
                                                               // in case more than 5 domains , you have to choose : any domain means : build.WithOrigins("*")...

    // instead of using "*", we can read a set of origins from db as we did top with origins list. so we can put origins.ToArray() instead of "*".
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy");

app.Run();