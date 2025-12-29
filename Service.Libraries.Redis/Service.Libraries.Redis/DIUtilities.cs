using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Libraries.Redis;

internal static class DIUtilities
{
    public static IServiceCollection AddSharedSingleton<TService1, TService2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TService1 : class
        where TService2 : class
        where TImplementation : class, TService1, TService2
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<TService1>(provider => provider.GetRequiredService<TImplementation>());
        services.AddSingleton<TService2>(provider => provider.GetRequiredService<TImplementation>());

        return services;
    }
}
