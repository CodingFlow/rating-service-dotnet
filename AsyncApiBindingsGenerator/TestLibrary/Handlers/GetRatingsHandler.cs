using TestProject.Application.Queries;
using TestProject.Application.QueryResponses;

namespace TestProject.Application.Handlers;

internal class GetRatingsHandler : IGetRatingsHandler
{

    public async Task<GetRatingsQueryResponse> Handle(GetRatingsQuery query)
    {
        return await Task.FromResult(new GetRatingsQueryResponse());
    }
}