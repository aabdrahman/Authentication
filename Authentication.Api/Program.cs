using System.Text.Json.Serialization;
using Authentication.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Serilog;

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, LoggerConfiguration) => 
{
   LoggerConfiguration.WriteTo.Console();
   LoggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => {
        c.RouteTemplate = "UserAuthentication/swagger/{documentname}/swagger.json";
    });
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/UserAuthentication/swagger/v1/swagger.json", "UserAuthentication");
        c.RoutePrefix = "UserAuthentication";
        
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
