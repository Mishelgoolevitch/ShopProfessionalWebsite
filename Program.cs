using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopProfessionalWebsite.Data;
using ShopProfessionalWebsite.Models;
using ShopProfessionalWebsite.Services;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 2.Enable Session State Services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Cart expires after 30 mins of inactivity
   // options.HttpOnly = true;                        // Security: Prevents XSS attacks from reading session cookie
    //options.IsEssential = true;                     // Essential for cart functionality to bypass GDPR cookie consent
});

// Stripe Configuration
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// 3. Register Custom Application Services
builder.Services.AddScoped<IShoppingCartService, ShopProfessionalWebsite.Services.ShoppingCartService>();


// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

//  Add HttpContextAccessor to access Session inside the Service layer
builder.Services.AddHttpContextAccessor();

// Session & Shopping Cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Custom Services for Dependency Injection
builder.Services.AddScoped<IProductService, ShopProfessionalWebsite.Services.ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShopProfessionalWebsite.Services.ShoppingCartService>();
builder.Services.AddScoped<IFileService, ShopProfessionalWebsite.Services.FileService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
app.UseAuthentication();
app.UseAuthorization();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedAsync(services);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");

//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
