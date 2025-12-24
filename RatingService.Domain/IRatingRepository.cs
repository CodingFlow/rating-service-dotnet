namespace RatingService.Domain;

public interface IRatingRepository : IRepository<Rating>
{
    IAsyncEnumerable<Rating> Find(IEnumerable<int> ratingIds);
    IAsyncEnumerable<Rating> FindAll();
    public Task Add(IEnumerable<Rating> rating);
    public Task<int> Save();
}
