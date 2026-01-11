namespace TestProject.Application.Commands;

public readonly struct DeleteRatingsCommand
{
    public IEnumerable<Guid> Ids { get; init; }
}