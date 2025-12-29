using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

var host = Environment.GetEnvironmentVariable("REDIS_SERVICE_HOST");
var port = Environment.GetEnvironmentVariable("REDIS_SERVICE_PORT");

var muxer = ConnectionMultiplexer.Connect($"{host}:{port}");
var db = muxer.GetDatabase();

var schema = new Schema()
            .AddTagField(new FieldName("$.Id", "id"), sortable: true)
            .AddNumericField(new FieldName("$.UserId", "userId"))
            .AddNumericField(new FieldName("$.ServiceId", "serviceId"))
            .AddNumericField(new FieldName("$.Score", "score"));

var indexCreated = db.FT().Create(
    "idx:ratings",
    new FTCreateParams()
        .On(IndexDataType.JSON)
        .Prefix("rating:"),
    schema
);