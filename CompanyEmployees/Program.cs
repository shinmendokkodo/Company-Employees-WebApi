using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utilities.ExceptionHandler;
using CompanyEmployees.Utilities.Mapper;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureApiBehavior();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.ConfigureControllers();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();