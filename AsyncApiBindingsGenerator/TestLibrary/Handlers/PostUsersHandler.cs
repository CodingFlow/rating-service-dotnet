using TestProject.Application.Commands;

namespace TestProject.Application.Handlers;

internal class PostUsersHandler : IPostRatingsHandler
{

    public async Task<string> Handle(PostRatingsCommand command)
    {
        return await Task.FromResult(string.Empty);
    }
}