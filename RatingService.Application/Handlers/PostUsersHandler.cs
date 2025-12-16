using RatingService.Application.Commands;

namespace RatingService.Application.Handlers;

internal class PostUsersHandler : IPostUsersHandler
{
    public string Handle(PostUsersCommand command)
    {
        Console.WriteLine($"Post request body username: {command.Items.First().Username}");

        return string.Empty;
    }
}