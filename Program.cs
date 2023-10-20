using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConverterRestApi.Data;
using System.Drawing.Text;
using ConverterRestApi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConverterRestApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConverterRestApiContext") ?? throw new InvalidOperationException("Connection string 'ConverterRestApiContext' not found.")));


var dbContext = builder.Services.BuildServiceProvider().GetService<ConverterRestApiContext>();
builder.Services.AddScoped<DataAccess>();
builder.Services.AddScoped<ConveterTools>();
//var te = dbContext?.DataUnits;
//DataAccess n = new(dbContext); 

//ConveterTools tool = new( n);
//tool.DoConvert("1","kilometer", "meter");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();