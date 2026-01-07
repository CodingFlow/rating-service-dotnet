using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using RatingService.Application.Commands;
using RatingService.Application.Handlers;
using RatingService.Application.Queries;
using RatingService.Application.QueryResponses;
using Service.Application.Common.Handlers;

namespace RatingService.Application;

public static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGetRatingsHandler, GetRatingsHandler>();
        services.AddScoped<IPostRatingsHandler, PostRatingsHandler>();
        services.AddScoped<IDeleteRatingsHandler, DeleteRatingsHandler>();

        services.AddScoped<IHandler<GetRatingsQuery, GetRatingsQueryResponse>, GetRatingsHandlerProxy>();
        services.AddScoped<IHandler<PostRatingsCommand, JsonObject>, PostRatingsHandlerProxy>();
        services.AddScoped<IHandler<DeleteRatingsQuery, JsonObject>, DeleteRatingsHandlerProxy>();

        return services;
    }
}
