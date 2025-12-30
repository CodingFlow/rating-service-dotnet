using Service.Application.Common.Handlers;

namespace TestProject.Application.Handlers;

public interface IPostRatingsHandler : IHandler<TestProject.Application.Commands.PostRatingsCommand, string>
{
}