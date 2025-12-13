using Microsoft.Extensions.Options;

namespace Service.Api.Common;

[OptionsValidator]
internal partial class ValidateNatsServiceOptions : IValidateOptions<NatsServiceOptions>
{
}