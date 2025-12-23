using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IPostRatingsHandler : IPostHandler<TestProject.Application.Commands.PostRatingsCommand, string>
{
}