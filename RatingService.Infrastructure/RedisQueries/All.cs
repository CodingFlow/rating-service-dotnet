namespace RatingService.Infrastructure.RedisQueries;

internal readonly record struct All : IRedisQuery
{
    private readonly string value = "*";

    public string Value => value;
    
    public All()
    {
    }

}