using MediatrCodeSideExamples.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatrCodeSideExamples.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        return services;
    }
}

