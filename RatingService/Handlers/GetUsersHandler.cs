namespace RatingService.Handlers;

internal class GetUsersHandler : IGetUsersHandler
{

    public User[] Handle()
    {
        return [
            new User { Username = "lionel57", Id = 27 },
            new User { Username = "catmaster", Id = 3 }
        ];
    }
}