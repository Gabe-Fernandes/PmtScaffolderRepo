using AutoGen.Data;
using AutoGen.Data.Models;
using AutoGen.Data.RepoInterfaces;
using AutoGen.Data.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
		builder.Configuration.GetConnectionString("AppDbContextConnection")));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
		.AddRoles<IdentityRole>()
		.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
{
	options.Cookie.Name = "Cookie";
	options.LoginPath = "/Account/Login";
	options.ExpireTimeSpan = TimeSpan.FromDays(1);
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.IsEssential = true;
	options.AccessDeniedPath = "/Error/AccessDenied";
});

builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 8;
	options.Password.RequiredUniqueChars = 0;
});

// PMT Landmark



// Misc. Services
// SignalR Landmark

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// Map Hubs Landmark

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); 
