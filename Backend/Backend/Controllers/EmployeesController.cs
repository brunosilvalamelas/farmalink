using Backend.DTOs.request;
using Backend.DTOs.response;
using Backend.Entities.Enums;
using Backend.Exceptions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// API controller for managing employee-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : BaseApiController
{
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initializes a new instance of the EmployeeController.
    /// </summary>
    /// <param name="employeeService">The employee service instance.</param>
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Creates a new employee and generates an authentication token.
    /// </summary>
    /// <param name="createEmployeeDto">The employee creation data.</param>
    /// <returns>An IActionResult containing the created employee data and token or error information.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateEmployeeResponseDto>>> CreateEmployee(
        [FromBody] CreateEmployeeRequestDto createEmployeeDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<CreateEmployeeResponseDto>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        try
        {
            var (createdEmployee, token) = await _employeeService.CreateEmployeeAsync(createEmployeeDto);

            var response = new CreateEmployeeResponseDto
            {
                Token = token,
                Name = createdEmployee.Name,
                Role = UserRole.Employee
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id },
                new ApiResponse<CreateEmployeeResponseDto>
                    { Success = true, Data = response, Message = "Employee registado" });
        }
        catch (ValidationException e)
        {
            return Conflict(new ApiResponse<CreateEmployeeResponseDto>
                { Success = false, Message = "Erros de validação", Errors = e.Errors });
        }
    }


    /// <summary>
    /// Retrieves all employees.
    /// </summary>
    /// <returns>An IActionResult containing the list of employee or error information.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<EmployeeResponseDto>>>> GetAllEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();

        var employeesResponse = employees.Select(employee => new EmployeeResponseDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            DeliveryLocation = employee.DeliveryLocation
        }).ToList();

        return Ok(new ApiResponse<List<EmployeeResponseDto>>
            { Message = "Employees encontrados.", Data = employeesResponse });
    }

    /// <summary>
    /// Retrieves a employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <returns>An IActionResult containing the employee data or error information.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> GetEmployeeById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return NotFound(new ApiResponse<EmployeeResponseDto>
                { Success = false, Message = "Não existe nenhum employee com esse id" });
        }


        var employeeResponse =
            new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DeliveryLocation = employee.DeliveryLocation
            };
        return Ok(new ApiResponse<EmployeeResponseDto> { Message = "Employee encontrado", Data = employeeResponse });
    }

    /// <summary>
    /// Updates an existing employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="updateEmployeeDto">The updated employee data.</param>
    /// <returns>An IActionResult containing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateEmployeeById(int id,
        [FromBody] UpdateEmployeeRequestDto updateEmployeeDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = GetModelStateErrors();
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Erros de validação",
                Errors = errors
            });
        }

        var updated = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);

        if (!updated)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum employee com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Os dados do employee foram atualizados" });
    }

    /// <summary>
    /// Deletes a employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>An IActionResult containing the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployeeById(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (!deleted)
        {
            return NotFound(
                new ApiResponse<bool> { Success = false, Message = "Não existe nenhum employee com esse id" });
        }

        return Ok(new ApiResponse<bool> { Message = "Employee removido" });
    }
}