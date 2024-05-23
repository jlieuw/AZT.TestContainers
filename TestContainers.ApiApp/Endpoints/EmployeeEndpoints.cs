namespace TestContainers.ApiApp.Endpoints;

using Microsoft.EntityFrameworkCore;
using Models;
using TestContainers.ApiApp.Infrastructure;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(WebApplication webApplication)
    {
        webApplication.MapGet("/employees", async (AppDbContext context) =>
        {
            return await context.Employees.ToListAsync();
        }).WithOpenApi();
    }
}
