namespace RatingService;

public readonly struct Rating
{
    public readonly required int Score { get; init; }

    public readonly required string ServiceName { get; init; }

    public readonly required string UserName { get; init; }

    public readonly required DateOnly Date { get; init; }
}
