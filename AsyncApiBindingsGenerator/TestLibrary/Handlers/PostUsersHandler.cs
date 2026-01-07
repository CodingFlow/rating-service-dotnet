using TestProject.Application.Commands;

namespace TestProject.Application.Handlers;

internal class PostUsersHandler : IPostRatingsHandler
{
    public Task Handle(PostRatingsCommand request)
    {
        return Task.CompletedTask;
    }
}