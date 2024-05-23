using Microsoft.EntityFrameworkCore;
using TestContainers.ApiApp.Endpoints;
using TestContainers.ApiApp.Infrastructure;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

        //Environment.SetEnvironmentVariable("DOCKER_HOST", "tcp://localhost:2375");
var _msSqlContainer = new  MsSqlBuilder().Build();
await _msSqlContainer.StartAsync();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(_msSqlContainer.GetConnectionString()));

// Rest of your code...
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

EmployeeEndpoints.MapEmployeeEndpoints(app);

app.Run();
