using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using ANNIE_SHOP.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Configure service of contex 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//configure authorization for users specifed
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "RequireAdminOrStaff",
        policy => policy.RequireRole("Administrador","Staff")

    );
});

//configue services of cookie 
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath= "/Account/Login";
        options.AccessDeniedPath="/Account/AccessDenied";
    });

// configure services for me
builder.Services.AddScoped<IProductoServices, ProductoServices>();
builder.Services.AddScoped<ICategoriaServices, CategoriaServices>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
