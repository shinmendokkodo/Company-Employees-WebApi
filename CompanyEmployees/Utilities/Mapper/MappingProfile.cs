using AutoMapper;
using Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utilities.Mapper;

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

        CreateMap<CompanyManipulateDto, Company>().ReverseMap();

        CreateMap<CompanyCreateDto, Company>();

        CreateMap<CompanyUpdateDto, Company>();

        CreateMap<EmployeeCreateDto, Employee>();

        CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();
    }
}