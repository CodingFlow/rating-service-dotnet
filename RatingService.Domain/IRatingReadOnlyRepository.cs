namespace RatingService.Domain;

public interface IRatingReadOnlyRepository
{
    IAsyncEnumerable<Rating> Find(IEnumerable<int> ratingIds);
    IAsyncEnumerable<Rating> FindAll();
}