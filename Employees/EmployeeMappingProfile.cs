using AutoMapper;

namespace EmployeeAPI.Employees;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>();
        CreateMap<Employee, GetEmployeeResponse>();
    }
}