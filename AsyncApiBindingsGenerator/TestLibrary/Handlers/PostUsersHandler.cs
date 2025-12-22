namespace TestProject.Application.Handlers;

internal class PostUsersHandler : IPostUsersHandler
{

    public async Task<string> Handle(IEnumerable<User> user)
    {
        return await Task.FromResult(string.Empty);
    }
}