using AutoMapper;
using Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(
                companyDto => companyDto.FullAddress,
                configurationExpression =>
                    configurationExpression.MapFrom(x => string.Join(' ', x.Address, x.Country))
            );

        CreateMap<Employee, EmployeeDto>();

        CreateMap<CompanyForManipulationDto, Company>().ReverseMap();

        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<CompanyForUpdateDto, Company>();

        CreateMap<EmployeeForCreationDto, Employee>();

        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
    }
}
