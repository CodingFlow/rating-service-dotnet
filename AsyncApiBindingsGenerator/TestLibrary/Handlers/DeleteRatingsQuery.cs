namespace TestProject.Application.Queries;

public readonly struct DeleteRatingsQuery
{
    public IEnumerable<Guid> Ids { get; init; }
}