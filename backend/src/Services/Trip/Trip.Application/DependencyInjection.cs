using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Common.Validation;

namespace Trip.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // 1. Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // 2. Scan and register Command & Query Handlers with Validation Decorators
        var commandHandlerInterface = typeof(ICommandHandler<,>);
        var queryHandlerInterface = typeof(IQueryHandler<,>);

        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false });

        foreach (var type in handlerTypes)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == commandHandlerInterface)
                {
                    var commandType = iface.GetGenericArguments()[0];
                    var resultType = iface.GetGenericArguments()[1];
                    var decoratorType = typeof(ValidationCommandHandlerDecorator<,>).MakeGenericType(commandType, resultType);

                    services.AddScoped(type);
                    services.AddScoped(iface, sp =>
                    {
                        var inner = sp.GetRequiredService(type);
                        var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
                        var validator = sp.GetService(validatorType);
                        return Activator.CreateInstance(decoratorType, inner, validator)!;
                    });
                }
                else if (iface.IsGenericType && iface.GetGenericTypeDefinition() == queryHandlerInterface)
                {
                    var queryType = iface.GetGenericArguments()[0];
                    var resultType = iface.GetGenericArguments()[1];
                    var decoratorType = typeof(ValidationQueryHandlerDecorator<,>).MakeGenericType(queryType, resultType);

                    services.AddScoped(type);
                    services.AddScoped(iface, sp =>
                    {
                        var inner = sp.GetRequiredService(type);
                        var validatorType = typeof(IValidator<>).MakeGenericType(queryType);
                        var validator = sp.GetService(validatorType);
                        return Activator.CreateInstance(decoratorType, inner, validator)!;
                    });
                }
            }
        }

        return services;
    }
}
