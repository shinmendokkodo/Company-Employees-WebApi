using CompanyEmployees.Formatters.Input;
using CompanyEmployees.Formatters.Output;
using CompanyEmployees.Presentation;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utilities.Links;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NLog;
using Repository;
using Service;
using Service.Contracts;
using Service.DataShaping;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy", policyBuilder => 
                policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination"))
        );
    }

    public static void ConfigureIISIntegration(this IServiceCollection services) => 
        services.Configure<IISOptions>(_ => { });

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        LogManager.Setup()
            .LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services) => 
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) => 
        services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureApiBehavior(this IServiceCollection services) => 
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

    private static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(options =>
        {
            options.OutputFormatters.Add(new CsvOutputFormatter());
        });

    public static void ConfigureControllers(this IServiceCollection services) =>
        services.AddControllers(options =>
        {
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true;
            options.InputFormatters.Insert(0, JsonPatchInputFormatter.GetJsonPatchInputFormatter());
        })
        .AddXmlDataContractSerializerFormatters()
        .AddCustomCsvFormatter()
        .AddApplicationPart(typeof(AssemblyReference).Assembly);

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(builder =>
            builder.UseSqlServer(configuration.GetConnectionString("SqlConnection"))
        );

    public static void ConfigureDataShaping(this IServiceCollection services)
    {
        services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
        services.AddScoped<IDataShaper<CompanyDto>, DataShaper<CompanyDto>>();
        services.AddScoped<IEmployeeLinks, EmployeeLinks>();
        services.AddScoped<ICompanyLinks, CompanyLinks>();
    }

    public static void ConfigureCustomMediaTypes(this IServiceCollection services) 
    { 
        services.Configure<MvcOptions>(config => 
        {
            var systemTextJsonOutputFormatter = config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();
            systemTextJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.shinmen.hateoas+json");

            var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();
            xmlOutputFormatter?.SupportedMediaTypes.Add("application/vnd.shinmen.hateoas+xml");
        });
    }

    public static void ConfigureFilters(this IServiceCollection services)
    {
        services.AddScoped<ValidationFilterAttribute>();
        services.AddScoped<ValidateMediaTypeAttribute>();
    }
}