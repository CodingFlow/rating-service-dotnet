namespace RatingService.Infrastructure.RedisQueries;

internal readonly record struct ByIds : IRedisQuery
{
    public string Value { get; private init; }

    public ByIds(IEnumerable<Guid> ids)
    {
        var escapedIds = ids.Select(id => id.ToString().Replace("-", @"\-"));
        var joinedIds = string.Join(" | ", escapedIds);
        var query = $@"@id:{{{joinedIds}}}";

        Value = query;
    }
}