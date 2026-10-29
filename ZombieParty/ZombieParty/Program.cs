using Microsoft.EntityFrameworkCore;
using ZombieParty.Models;
using ZombieParty.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ZombiePartyDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies()
);
builder.Services.AddSingleton<IImageFileManager, ImageFileManager>();
builder.Services.AddScoped<IZombiePartyDbContextSeed, ZombiePartyDbContextSeedDev>();

switch (builder.Environment.EnvironmentName)
{
    case "Development":
        builder.Services.AddScoped<IZombiePartyDbContextSeed, ZombiePartyDbContextSeedDev>(); break;
    case "StagingPerf":
        builder.Services.AddScoped<IZombiePartyDbContextSeed, ZombiePartyDbContextSeedStagingPerf>(); break;
    default:
        builder.Services.AddScoped<IZombiePartyDbContextSeed, ZombiePartyDbContextSeedProd>(); break;
}


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{

    var scopedProvider = scope.ServiceProvider;
    try
    {
        var zombiePartyDbContextSeed = scopedProvider.GetRequiredService<IZombiePartyDbContextSeed>();
        await zombiePartyDbContextSeed.SeedAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
