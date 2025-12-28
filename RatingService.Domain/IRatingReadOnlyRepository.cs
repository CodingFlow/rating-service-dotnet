using DecoratorGenerator;

namespace RatingService.Domain;

[Decorate]
public interface IRatingReadOnlyRepository
{
    IAsyncEnumerable<Rating> Find(IEnumerable<Guid> ratingIds);
    IAsyncEnumerable<Rating> FindAll();
}