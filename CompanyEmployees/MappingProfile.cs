using AutoMapper;
using Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees;

public class MappingProfile : Profile
{
    public MappingProfile() 
    { 
        CreateMap<Company, CompanyDto>()
            .ForMember(companyDto => companyDto.FullAddress, configurationExpression => 
                configurationExpression.MapFrom(x => string.Join(' ', x.Address, x.Country))); 
        
        CreateMap<Employee, EmployeeDto>();
        
        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<EmployeeForCreationDto, Employee>();
    }
}