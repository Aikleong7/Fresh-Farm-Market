using Fresh_Farm_Market.Model;
using Fresh_Farm_Market.Models;
using Fresh_Farm_Market.Service;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<UserDbContext>();
builder.Services.AddScoped<PasswordHistoryService>();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
});
builder.Services.AddTransient<reCaptchaService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});
builder.Services.AddSession(options =>
{
    
    options.Cookie.Name = "FreshFarmMarket";
    options.IdleTimeout = TimeSpan.FromMinutes(1);
});
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/login");
builder.Services.AddDataProtection();


builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       
        //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    }).AddCookie(options =>
    {
        options.LoginPath = "/Login";
    })
    .AddGoogle(options =>
{
    options.SignInScheme = IdentityConstants.ExternalScheme;
  
    options.ClientId = "34536395750-4qemn61eaj75l8h9mapfv97095l0irhi.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
