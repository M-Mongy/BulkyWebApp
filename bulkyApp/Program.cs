using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bulky.Data.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Microsoft.CodeAnalysis.Options;
using Bulky.DataAccess.DBInitializer;
using Bulky.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<StripSettings>(builder.Configuration.GetSection("Stripe"));

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "1243655363910858";
    option.AppSecret = "f76f7ea4fd9f968bfb51de562a9b5f6e";
});
// This service is required to enable session state. It sets up an in-memory cache.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IDBInitializer, DbInitializer>();

builder.Services.AddRazorPages();

// CORRECTED: Typo in interface name IUnitOfWork
builder.Services.AddScoped<IuintOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

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

// REMOVED: The duplicate and incorrect Stripe configuration was removed from here.

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
SeeddataBase();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer2}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeeddataBase()
{
    using (var scope = app.Services.CreateScope()) {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        dbInitializer.Initialize();
    
    }
}