namespace RatingService.Domain;

public partial class Rating : IAggregateRoot
{
    public RatingId Id { get; set; } = RatingId.From(Guid.CreateVersion7());

    public int UserId { get; set; }

    public int ServiceId { get; set; }

    public int Score { get; set; }
}
