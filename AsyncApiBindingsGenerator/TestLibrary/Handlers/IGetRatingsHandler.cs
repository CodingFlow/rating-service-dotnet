using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IGetRatingsHandler : IGetHandler<TestProject.Application.QueryResponses.GetRatingsQueryResponse>
{
}