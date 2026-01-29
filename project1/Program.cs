using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using project1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// اتصال به دیتابیس
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(
        connectionString: "Data Source=.;" +
        "Initial Catalog = SE_Site_DB;" +
        "Trusted_Connection = True;" +
        "TrustServerCertificate = True;"
    );
});

// احراز هویت با کوکی
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(10);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
    options.AddPolicy("Librarian", policy => policy.RequireClaim("Role", "Librarian", "Admin"));
    options.AddPolicy("User", policy => policy.RequireClaim("Role", "User", "Admin", "Librarian"));
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();

app.UseStatusCodePages("text/plain", "Unfortunately,this page could not be found!!!");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


