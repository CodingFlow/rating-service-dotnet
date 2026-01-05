using CodingFlow.FluentValidation.Validators;
using CodingFlow.FluentValidation.VogenExtensions;
using Vogen;
using static CodingFlow.FluentValidation.Validations;

namespace RatingService.Domain;

[Instance("Minimum", 0f)]
[Instance("Maximum", 5f)]
[ValueObject<float>(throws: typeof(InvalidScoreException))]
public readonly partial record struct Score
{
    private static Validation Validate(float input)
    {
        return RuleFor(input)
            .BetweenInclusive(Minimum.Value, Maximum.Value)
            .VogenResult();
    }
}
