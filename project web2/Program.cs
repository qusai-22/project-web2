using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using project_web2.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<project_web2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("project_web2Context") ?? throw new InvalidOperationException("Connection string 'project_web2Context' not found.")));
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=usersaccounts}/{action=login}/{id?}");

app.Run();
