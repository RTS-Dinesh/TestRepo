using System.Reflection;
using System.Linq;
using MediatR;
using MediatrCodeSideExamples.Application.Common.Behaviors;
using MediatrCodeSideExamples.Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace MediatrCodeSideExamples.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));

        RegisterRequestValidators(services, assembly);
        return services;
    }

    private static void RegisterRequestValidators(IServiceCollection services, Assembly assembly)
    {
        var validatorInterface = typeof(IRequestValidator<>);

        var registrations = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(
                implementation => implementation
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterface)
                    .Select(service => new { service, implementation }));

        foreach (var registration in registrations)
        {
            services.AddTransient(registration.service, registration.implementation);
        }
    }
}

