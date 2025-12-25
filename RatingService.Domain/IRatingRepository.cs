namespace RatingService.Domain;

public interface IRatingRepository : IRepository<Rating>
{
    IAsyncEnumerable<Rating> Find(IEnumerable<int> ratingIds);
    IAsyncEnumerable<Rating> FindAll();
    public Task Add(IEnumerable<Rating> rating);
    Task<int> Delete(IEnumerable<int> ratingIds);
    Task<int> DeleteAll();
    public Task<int> Save();
}
