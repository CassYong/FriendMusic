using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FriendMusic.Data;
using FriendMusic.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");
builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AuthDbContext>();



// Register Syncfusion license if needed
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Your Syncfusion License Key");

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure this line is present to serve static files from wwwroot

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers and razor pages
app.MapControllerRoute(
    name: "profile",
    pattern: "Profile/{action=Index}/{id?}",
    defaults: new { controller = "Profile", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "music",
    pattern: "Music/{action=Library}/{id?}",
    defaults: new { controller = "Music", action = "Library" });

app.MapControllerRoute(
    name: "music-delete",
    pattern: "Music/Delete/{id?}",
    defaults: new { controller = "Music", action = "Delete" });

app.MapControllerRoute(
    name: "social",
    pattern: "{controller=Social}/{action=Index}/{id?}",
    defaults: new { controller = "Social", action = "Index" });

app.MapControllerRoute(
    name: "social",
    pattern: "Social/{action=Index}/{id?}",
    defaults: new { controller = "Social", action = "Index" });



app.MapRazorPages();

app.Run();