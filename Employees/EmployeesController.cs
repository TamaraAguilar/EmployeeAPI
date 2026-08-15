using AutoMapper;
using EmployeeAPI.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Employees;

public class EmployeesController : BaseController
{
    private readonly IRepository<Employee> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IRepository<Employee> repository, IMapper mapper, ILogger<EmployeesController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var employees = _mapper.Map<List<GetEmployeeResponse>>(_repository.GetAll());

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployeeById(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }
        
        var employeeResponse = _mapper.Map<Employee, GetEmployeeResponse>(employee);
        
        return Ok(employeeResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest employeeRequest)
    {
        var validationResults = await ValidateAsync(employeeRequest);
        if (!validationResults.IsValid)
        {
            return ValidationProblem(validationResults.ToModelStateDictionary());
        }

        var newEmployee = _mapper.Map<CreateEmployeeRequest, Employee>(employeeRequest);

        _repository.Create(newEmployee);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Id }, newEmployee);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest employeeRequest)
    {
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
        
        var existingEmployee = _repository.GetById(id);
        if (existingEmployee == null)
        {
            return NotFound();
        }
        
        _logger.LogDebug("Updating employee details for ID: {EmployeeId}", id);
        existingEmployee = _mapper.Map<UpdateEmployeeRequest, Employee>(employeeRequest);

        try
        {
            _repository.Update(existingEmployee);
            _logger.LogInformation("Employee with ID: {EmployeeId} successfully updated", id);
            return Ok(existingEmployee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
            return StatusCode(500, "An error occurred while updating the employee");
        }
    }
}