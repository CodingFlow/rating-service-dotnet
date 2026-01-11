using Microsoft.Extensions.Options;

namespace Service.Api.Common.NatsConnectionServices;

[OptionsValidator]
internal partial class ValidateNatsServiceOptions : IValidateOptions<NatsServiceOptions>
{
}