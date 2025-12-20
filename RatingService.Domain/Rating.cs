namespace RatingService.Domain;

public partial class Rating : IAggregateRoot
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public int ServiceId { get; set; }

    public int Score { get; set; }
}
