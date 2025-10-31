using Backend.DTOs.request;
using Backend.Entities;

namespace Backend.Services;

/// <summary>
/// Interface for employee-related service operations.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Creates a new employee in the database.
    /// </summary>
    /// <param name="employeeDto">The data to create a new employee.</param>
    /// <returns>A ServiceResult containing the created employee or validation errors.</returns>
    Task<(Employee employee, string token)> CreateEmployeeAsync(CreateEmployeeRequestDto createEmployeeDto);

    /// <summary>
    /// Retrieves all employees from the database.
    /// </summary>
    /// <returns>A ServiceResult containing the list of employee.</returns>
    Task<List<Employee>> GetAllEmployeesAsync();

    /// <summary>
    /// Retrieves an employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <returns>A ServiceResult containing the employee or a not found result.</returns>
    Task<Employee?> GetEmployeeByIdAsync(int id);

    /// <summary>
    /// Updates an existing employee with new data.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="updateEmployeeDto">The updated employee data.</param>
    /// <returns>A ServiceResult indicating the success of the update operation.</returns>
    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequestDto updateEmployeeDto);

    /// <summary>
    /// Deletes an employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>A ServiceResult indicating the success of the delete operation.</returns>
    Task<bool> DeleteEmployeeAsync(int id);
}