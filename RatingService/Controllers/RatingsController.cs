using Microsoft.AspNetCore.Mvc;

namespace RatingService.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingsController(ILogger<RatingsController> logger) : ControllerBase
{
    [HttpGet(Name = "GetRatings")]
    public IEnumerable<Rating> Get()
    {
        return
        [
            new Rating
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Score = 0,
                ServiceName = "Paypal",
                UserName = "John Kibby"
            },
            new Rating
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Score = 2,
                ServiceName = "Paypal",
                UserName = "Emily Lawson"
            }
        ];
    }
}
