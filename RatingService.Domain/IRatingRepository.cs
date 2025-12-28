using DecoratorGenerator;

namespace RatingService.Domain;

[Decorate]
public interface IRatingRepository : IRepository<Rating>
{
    IAsyncEnumerable<Rating> Find(IEnumerable<Guid> ratingIds);
    IAsyncEnumerable<Rating> FindAll();
    public Task Add(IEnumerable<Rating> ratings);
    Task<int> Delete(IEnumerable<Guid> ratingIds);
    Task<int> DeleteAll();
    public Task<int> Save();
}
