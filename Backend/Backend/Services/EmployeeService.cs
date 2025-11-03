using Backend.Data;
using Backend.DTOs.request;
using Backend.Entities;
using Backend.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Service for managing employee operations.
/// Implements <see cref="IEmployeeService"/>.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly DataContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the EmployeeService.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userService">The user service for validation.</param>
    public EmployeeService(DataContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Retrieves an employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee.</param>
    /// <returns>The employee if found, or null otherwise.</returns>
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all employees from the database.
    /// </summary>
    /// <returns>A list of all employees.</returns>
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="dto">The data to create the employee.</param>
    /// <returns>The created employee.</returns>
    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequestDto dto)
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

        _context.Employees.Add(newEmployee);
        await _context.SaveChangesAsync();

        return newEmployee;
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="updateEmployeeDto">The updated data.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
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

    /// <summary>
    /// Deletes an employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>True if the deletion was successful, otherwise false.</returns>
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}
