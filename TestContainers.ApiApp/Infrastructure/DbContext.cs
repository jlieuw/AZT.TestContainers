namespace TestContainers.ApiApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Models;
using TestContainers.ApiApp.Infrastructure.Configurations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Name = "Employee 1",
                Department = "Department 1",
                Email = "employee1@test.com",
                Phone = "1234567890"
            },
            new Employee
            {
                Id = 2,
                Name = "Employee 2",
                Department = "Department 2",
                Email = "employee2@test.com",
                Phone = "1234567891"
            },
            new Employee
            {
                Id = 3,
                Name = "Employee 3",
                Department = "Department 3",
                Email = "employee3@test.com",
                Phone = "1234567892"
            },
            new Employee
            {
                Id = 4,
                Name = "Employee 4",
                Department = "Department 4",
                Email = "employee4@test.com",
                Phone = "1234567893"
            },
            new Employee
            {
                Id = 5,
                Name = "Employee 5",
                Department = "Department 5",
                Email = "employee5@test.com",
                Phone = "1234567894"
            }
        );
    }
}
