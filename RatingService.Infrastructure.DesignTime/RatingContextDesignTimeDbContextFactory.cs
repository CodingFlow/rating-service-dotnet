using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Libraries.Redis;

namespace RatingService.Infrastructure.DesignTime;

internal class RatingContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<RatingContext>
{
    public RatingContext CreateDbContext(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddRedis(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        using var host = builder.Build();

        var serviceScope = host.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        return provider.GetRequiredService<RatingContext>();
    }
}
