using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
Host.CreateDefaultBuilder(args)
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext();
        })
.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Program>();
        });

builder.Services.AddApiVersioning(options =>
{
options.DefaultApiVersion = new ApiVersion(1, 0);
options.AssumeDefaultVersionWhenUnspecified = true;
options.ReportApiVersions = true;



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog(dispose: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name V1");
        c.RoutePrefix = string.Empty; // Set the Swagger UI at the root URL
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
