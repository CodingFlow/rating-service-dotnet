
namespace Service.Api.Common;

public interface IQueryParameterParser
{
    (IEnumerable<Guid>, IEnumerable<ValidationError>) ParseGuid(IEnumerable<Guid> originalValues, Dictionary<string, string> queryParameters, string queryParameterKey);
}