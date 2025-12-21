namespace RatingService.Domain;

public interface IRatingRepository : IRepository<Rating>
{
    Task<Rating[]> Find(int[] ratingIds);
    Task<Rating[]> FindAll();
    public Task Add(Rating[] rating);
    public Task<int> Save();
}
