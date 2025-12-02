namespace RatingService.Handlers;

internal interface IPostUsersHandler
{
    string HandlePostUsersAsync(User user);
}