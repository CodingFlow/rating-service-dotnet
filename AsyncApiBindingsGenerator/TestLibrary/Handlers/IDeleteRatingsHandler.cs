using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IDeleteRatingsHandler : IDeleteHandler<Queries.DeleteRatingsQuery>
{
}