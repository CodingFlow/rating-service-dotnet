using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RatingService.Infrastructure.DesignTime;

internal class RatingContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<RatingContext>
{
    public RatingContext CreateDbContext(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddInfrastructureServices(builder.Configuration);

        using var host = builder.Build();

        var serviceScope = host.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        return provider.GetRequiredService<RatingContext>();
    }
}
