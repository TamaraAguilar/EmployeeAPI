using EmployeeAPI;
using EmployeeAPI.Abstractions;
using EmployeeAPI.Employees;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "John", LastName = "Doe",
        Benefits = new List<EmployeeBenefits>
        {
            new EmployeeBenefits { BenefitType = BenefitType.Health, Cost = 100 },
            new EmployeeBenefits { BenefitType = BenefitType.Dental, Cost = 50 }
        } },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe" }
};

var employeeRepository = new EmployeeRepository();
foreach (var e in employees)
{
    employeeRepository.Create(e);
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IRepository<Employee>>(employeeRepository);
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(cfg => { }, typeof(EmployeeMappingProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data source=employees.db");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();