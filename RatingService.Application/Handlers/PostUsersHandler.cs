namespace RatingService.Application.Handlers;

internal class PostUsersHandler : IPostUsersHandler
{

    public string Handle(User[] user)
    {
        Console.WriteLine($"Post request body username: {user.First().Username}");

        return string.Empty;
    }
}