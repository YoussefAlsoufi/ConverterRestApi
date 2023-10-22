using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConverterRestApi.Data;
using System.Drawing.Text;
using ConverterRestApi;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConverterRestApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConverterRestApiContext") ?? throw new InvalidOperationException("Connection string 'ConverterRestApiContext' not found.")));


var dbContext = builder.Services.BuildServiceProvider().GetService<ConveterTools>();

//var origins = new List<string>();
//if (dbContext != null)
//{
//    var originData = dbContext.Origins.Where(item => item.IsActive == true).ToList();
//    if (originData.Any() && originData != null)
//    {
//        originData.ForEach(item =>
//        origins.Add(item.OriginName));
//    }
//}


builder.Services.AddScoped<DataAccess>();
builder.Services.AddScoped<ConveterTools>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy");

app.Run();