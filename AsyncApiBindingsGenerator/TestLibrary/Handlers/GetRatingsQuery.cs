namespace TestProject.Application.Queries;

public readonly struct GetRatingsQuery
{
    
    public IEnumerable<int> Ids { get; init; }
}