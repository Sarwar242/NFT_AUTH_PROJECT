using Microsoft.EntityFrameworkCore;
using ServerSideApplication.DbConnection;
using ServerSideApplication.Service;
<<<<<<< HEAD
using ServerSideApplication.Service.EmployeeProfile;
=======
using ServerSideApplication.Service.AuthorizerGroup;
>>>>>>> dev

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbConnection>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IGroceryService, GroceryService>();
builder.Services.AddScoped<IAuthorizerGroupService,AuthorizerGroupService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IEmployeeProfileService, EmployeeProfileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        build =>
        {
            build.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
