namespace RatingService.Domain;

internal class InvalidScoreException : Exception
{
    public InvalidScoreException()
    {
    }

    public InvalidScoreException(string? message) : base(message)
    {
    }

    public InvalidScoreException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}