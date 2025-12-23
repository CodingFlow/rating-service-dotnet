using TestProject.Application.QueryResponses;

namespace TestProject.Application.Handlers;

internal class GetRatingsHandler : IGetRatingsHandler
{

    public async Task<GetRatingsQueryResponse> Handle()
    {
        return await Task.FromResult(new GetRatingsQueryResponse());
    }
}