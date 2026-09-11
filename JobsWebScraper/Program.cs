using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JobsWebScraper.Data;
using JobsWebScraper.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MyAPIContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
//builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IJobsRepository, JobsRepository>();
builder.Services.AddScoped<IAutomationService, AutomationService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder.WithOrigins("https://ex-machin.github.io/WebScraper-Frontend/"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
