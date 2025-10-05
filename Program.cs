using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WordGuessGame.Data;
using WordGuessGame.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // ไม่ต้องยืนยันอีเมล
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GameService>();

var app = builder.Build();

// Configure URL binding - only for production deployment (Docker/Cloud)
if (app.Environment.IsProduction())
{
    app.Urls.Add("http://0.0.0.0:8080");
}

app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // สร้าง /data directory ถ้ายังไม่มี (สำหรับ production เท่านั้น)
    if (app.Environment.IsProduction())
    {
        Directory.CreateDirectory("/data");
    }
    
    // Run database migrations
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    // Seed data
    SeedData.Initialize(services);
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

app.Run();
