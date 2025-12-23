namespace RatingService.Domain;

public interface IRatingReadOnlyRepository
{
    IAsyncEnumerable<Rating> Find(int[] ratingIds);
    IAsyncEnumerable<Rating> FindAll();
}