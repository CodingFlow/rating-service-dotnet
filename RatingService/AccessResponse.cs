namespace RatingService;

public class AccessResponse
{
    public required Result result { get; set; }
}

public class Result
{
    public bool get { get; set; }
    public required string call { get; set; }
}
