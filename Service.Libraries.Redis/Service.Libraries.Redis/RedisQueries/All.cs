namespace Service.Libraries.Redis.RedisQueries;

public readonly record struct All : IRedisQuery
{
    private readonly string value = "*";

    public string Value => value;
    
    public All()
    {
    }

}