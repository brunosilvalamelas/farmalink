using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly DataContext _context;
    private readonly IUserService _userService;

    public EmployeeService(DataContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<(Employee employee, string token)> CreateEmployeeAsync(CreateEmployeeRequestDto dto)
    {
        var fields = new Dictionary<string, string>
        {
            { "Email", dto.Email },
            { "PhoneNumber", dto.PhoneNumber }
        };
        await _userService.ValidateDuplicatesAsync<User>(fields);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        Employee newEmployee = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = hashedPassword,
            PhoneNumber = dto.PhoneNumber,
            DeliveryLocation = dto.DeliveryLocation,
            Role = UserRole.Employee,
        };
        await _context.Employees.AddAsync(newEmployee);
        await _context.SaveChangesAsync();
        var token = _userService.GenerateToken(newEmployee.Id, newEmployee.Role);
        return (newEmployee, token);
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequestDto updateEmployeeDto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return false;
        }

        employee.Name = updateEmployeeDto.Name;
        employee.DeliveryLocation = updateEmployeeDto.DeliveryLocation;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        _context.SaveChanges();
        return true;
    }
}