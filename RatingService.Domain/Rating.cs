namespace RatingService.Domain;

public class Rating : IAggregateRoot
{
    public RatingId Id { get; init; } = RatingId.From(Guid.CreateVersion7());

    public required UserId UserId { get; init; }

    public required ServiceId ServiceId { get; init; }

    public required Score Score { get; init; }
}
