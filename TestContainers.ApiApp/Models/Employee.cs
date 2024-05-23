namespace TestContainers.ApiApp.Models;

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Department { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}
