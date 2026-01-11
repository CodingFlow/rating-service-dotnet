using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IGetRatingsHandler : IHandler<Queries.GetRatingsQuery, QueryResponses.GetRatingsQueryResponse>
{
}