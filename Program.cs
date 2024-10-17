using FastReport.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsStore.Binder;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Interfaces;
using SportsStore.Models;
using SportsStore.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.Configure<IdentityOptions>(options =>
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

//Mail shipping
EmailSettings emailSettings = new EmailSettings
{
    WriteAsFile = bool.Parse(builder.Configuration.GetSection("AppSettings:Email.WriteAsFile").Value ?? "false")
};
//builder.Services.AddTransient<IOrderProcessor, EmailOrderProcessor>();
builder.Services.AddTransient<IOrderProcessor>(provider =>
     new EmailOrderProcessor(emailSettings));

//Payment Services
builder.Services.AddTransient<IBraintreeService, BraintreeService>();

//Reporting
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
builder.Services.AddFastReport();

//Model Binding
builder.Services.AddMvc(o =>
{
    // adds custom binder at first place
    o.ModelBinderProviders.Insert(0, new CartModelBinderProvider());
}).AddRazorRuntimeCompilation();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".mySportsStore.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

//builder.Services.AddDistributedSqlServerCache(options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    options.SchemaName = "dbo";
//    options.TableName = "StoreCache";
//    options.DefaultSlidingExpiration = TimeSpan.FromSeconds(10);
//});

var app = builder.Build();

// Configuring User Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
        await ContextSeed.SeedRolesAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseDeveloperExceptionPage();
//app.UseDatabaseErrorPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//enable session before MVC
app.UseSession();

//Report
app.UseFastReport();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();