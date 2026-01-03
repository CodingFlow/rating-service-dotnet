using FluentValidation;
using TestProject.Queries;

namespace TestLibrary;

internal class GetRatingsQueryValidator : AbstractValidator<GetRatingsQuery>
{
    public GetRatingsQueryValidator()
    {
        //RuleForEach(x => x.Ids).Must(item => item.)
    }
}
