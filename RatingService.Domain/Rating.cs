namespace RatingService.Domain;

public partial class Rating : IAggregateRoot
{
    public RatingId Id { get; init; } = RatingId.From(Guid.CreateVersion7());

    public UserId UserId { get; init; }

    public ServiceId ServiceId { get; init; }

    public Score Score { get; init; }
}
