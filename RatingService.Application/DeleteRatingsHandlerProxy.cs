using System.Text.Json.Nodes;
using RatingService.Application.Commands;
using RatingService.Application.Handlers;
using Service.Application.Common.Handlers;

namespace RatingService.Application;

internal class DeleteRatingsHandlerProxy(IDeleteRatingsHandler handler) : IHandler<DeleteRatingsCommand, JsonObject>
{
    async Task<JsonObject> IHandler<DeleteRatingsCommand, JsonObject>.Handle(DeleteRatingsCommand request)
    {
        await handler.Handle(request);
        return [];
    }
}
