namespace RatingService.Domain;

public interface IRatingRepository : IRepository<Rating>
{
    public Task Add(Rating[] rating);

    public Task<int> Save();
}
