using Microsoft.Extensions.DependencyInjection;

namespace Test.Common.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static bool RemoveRegisteredService<TService>(this IServiceCollection services)
    {
        var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        return serviceDescriptor is not null && services.Remove(serviceDescriptor);
    }

    internal static IServiceCollection ReplaceService<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
        where TImplementation : class, TService
    {
        var isRemoved = services.RemoveRegisteredService<TService>();
        if (!isRemoved)
        {
            return services;
        }

        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));

        return services;
    }

    internal static IServiceCollection ReplaceService<TService>(
        this IServiceCollection services,
        TService instance,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TService : class
    {
        var isRemoved = services.RemoveRegisteredService<TService>();
        if (!isRemoved)
        {
            return services;
        }

        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(_ => instance);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(_ => instance);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(_ => instance);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Invalid service lifetime");
        }

        return services;
    }
}
