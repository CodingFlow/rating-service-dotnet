using RatingService.Application.Handlers;
using RatingService.Application.Queries;
using RatingService.Application.QueryResponses;
using Service.Application.Common.Handlers;

namespace RatingService.Application;

internal class GetRatingsHandlerProxy(IGetRatingsHandler handler) : IHandler<GetRatingsQuery, GetRatingsQueryResponse>
{
    async Task<GetRatingsQueryResponse> IHandler<GetRatingsQuery, GetRatingsQueryResponse>.Handle(GetRatingsQuery request)
    {
        return await handler.Handle(request);
    }
}
