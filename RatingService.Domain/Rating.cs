using Validly;
using Validly.Extensions.Validators.Common;
using Validly.Extensions.Validators.Numbers;

namespace RatingService.Domain;

[Validatable]
public partial class Rating
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Between(1, 10)]
    public int Score { get; set; }
}
