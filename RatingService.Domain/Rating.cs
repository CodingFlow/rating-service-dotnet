namespace RatingService.Domain;

public partial class Rating : IAggregateRoot
{
    public Guid Id { get; set; }
    
    public int UserId { get; set; }

    public int ServiceId { get; set; }

    public int Score { get; set; }

    public Rating()
    {
        if (Id == Guid.Empty)
        {
            Id = Guid.CreateVersion7();
        }
    }
}
