namespace RatingService.Application.Handlers;

internal class PostUsersHandler : IPostUsersHandler
{

    public string Handle(User[] users)
    {
        Console.WriteLine($"Post request body username: {users.First().Username}");

        return string.Empty;
    }
}