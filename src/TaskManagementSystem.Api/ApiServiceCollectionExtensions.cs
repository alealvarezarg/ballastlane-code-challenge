using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using TaskManagementSystem.Api.ExceptionHandling;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.Validation;
using TaskManagementSystem.Infrastructure.Authentication;

namespace TaskManagementSystem.Api;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManagementApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationSettings = configuration
            .GetSection(JwtAuthenticationSettings.SectionName)
            .Get<JwtAuthenticationSettings>() ?? new JwtAuthenticationSettings();

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(entry => entry.Value?.Errors.Count > 0)
                        .SelectMany(entry => entry.Value!.Errors.Select(error =>
                            new ValidationErrorDto(
                                entry.Key,
                                string.IsNullOrWhiteSpace(error.ErrorMessage)
                                    ? "The supplied value is invalid."
                                    : error.ErrorMessage)))
                        .ToArray();

                    var response = new ErrorResponseDto
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "One or more validation errors occurred.",
                        Errors = errors,
                        TraceId = context.HttpContext.TraceIdentifier
                    };

                    return new BadRequestObjectResult(response);
                };
            });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });
        services.AddValidatorsFromAssemblyContaining<CreateManagementTaskRequestDtoValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddEndpointsApiExplorer();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = authenticationSettings.Issuer,
                    ValidAudience = authenticationSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.SigningKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddAuthorization();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Supply a bearer token to call protected endpoints."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
