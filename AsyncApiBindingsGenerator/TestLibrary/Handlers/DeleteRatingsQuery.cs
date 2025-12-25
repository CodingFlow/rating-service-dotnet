namespace TestProject.Application.Queries;

public readonly struct DeleteRatingsQuery
{
    public IEnumerable<int> Ids { get; init; }
}