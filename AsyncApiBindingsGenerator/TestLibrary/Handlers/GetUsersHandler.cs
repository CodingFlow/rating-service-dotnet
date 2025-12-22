namespace TestProject.Application.Handlers;

internal class GetUsersHandler : IGetUsersHandler
{

    public async Task<IEnumerable<User>> Handle()
    {
        return await Task.FromResult(new User[1]);
    }
}