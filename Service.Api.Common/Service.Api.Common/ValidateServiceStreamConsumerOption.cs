using Microsoft.Extensions.Options;

namespace Service.Api.Common;

[OptionsValidator]
internal partial class ValidateServiceStreamConsumerOptions : IValidateOptions<ServiceStreamConsumerOptions>
{
}