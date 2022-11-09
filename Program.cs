using FlashCards.Authorization;
using FlashCards.Data;
using FlashCards.Data.Services;
using FlashCards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FlashCards.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<ICardsService, CardsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddAuthorization(options => options.AddPolicy("MustBeCardSetOwner", policy =>
    policy.Requirements.Add(new MustBeCardSetOwnerRequirement())));
builder.Services.AddScoped<IAuthorizationHandler, MustBeCardSetOwnerHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.SeedDbData(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
