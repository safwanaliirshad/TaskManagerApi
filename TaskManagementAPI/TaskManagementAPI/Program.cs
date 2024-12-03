using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;
using EntityModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb"));
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseInMemoryDatabase("IdentityDb"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TaskManagetmentapi",
            ValidAudience = "TaskManagetmentapi",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("58d5g8df5g5d5f47f7g8d5fg54g8d4gf54d84fv5d4f84g5d48f45g4f54f8v45d4d54f84v54d8f44v"))
        };
    });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
    // Seed a user
    var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = "admin", Email = "admin@gmail.com" };
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    var normalUser = await userManager.FindByEmailAsync("user@gmail.com");
    if (normalUser == null)
    {
        normalUser = new IdentityUser { UserName = "user", Email = "user@gmail.com" };
        await userManager.CreateAsync(normalUser, "User123!");
        await userManager.AddToRoleAsync(normalUser, "User");
    }
    var context1 = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context1.Tasks.AddRange(
        new EntityModel.Task { Id = 1, Title = "Task 1", AssignedUserId = normalUser.Id, Description = "test desc 1", Status = "InProgress" },
        new EntityModel.Task { Id = 2, Title = "Task 2", AssignedUserId = normalUser.Id, Description = "test desc 2", Status = "InProgress" },
        new EntityModel.Task { Id = 3, Title = "Task 3", AssignedUserId = normalUser.Id, Description = "test desc 3", Status = "InProgress" }
    );
    context1.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();

app.Run();
