using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register ApplicationDbContext with the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()  // This enables role management
    .AddEntityFrameworkStores<ApplicationDbContext>();




builder.Services.AddControllersWithViews();

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

// Ensure authentication and authorization middleware are called
app.UseAuthentication();
app.UseAuthorization();

// Ensure roles are created and an admin user is seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeedRolesAndAdminUserAsync(roleManager, userManager);
}

// Route configuration: Set Home as the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();  // Necessary for Identity pages

app.Run();

// Method to seed roles and admin user
async Task SeedRolesAndAdminUserAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    // Define the roles you want to create
    string[] roleNames = { "Admin" };

    foreach (var roleName in roleNames)
    {
        // Check if the role exists, if not, create it
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create an admin user if not already created
    string adminEmail = "admin@backoffice.com";
    string adminPassword = "AdminPassword123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
