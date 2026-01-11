namespace TestProject.Application.Queries;

public readonly struct GetRatingsQuery
{
    public IEnumerable<Guid> Ids { get; init; }
}