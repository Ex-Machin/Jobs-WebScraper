using JobsWebScraper.Data;
using JobsWebScraper.Web.Components;
using JobsWebScraper.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// A Blazor Server circuit is long-lived, so a scoped DbContext would live for the whole
// session and could be used concurrently by overlapping renders. Create one per operation.
builder.Services.AddDbContextFactory<MyAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddBlazorBootstrap();

builder.Services.AddHttpClient<JobsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["JobsApi:BaseUrl"]!);
    // Scraping drives Selenium across four sites, so it runs for minutes.
    client.Timeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
