namespace Service.Api.Common;

public class Result<T>
    where T : struct
{
    public T Value { get; init; }

    public Response<ValidationError> Error { get; set; }

    public bool IsValid => !EqualityComparer<T>.Default.Equals(Value, default);

    public static implicit operator Result<T>(T value)
    {
        return new Result<T> { Value = value };
    }
}