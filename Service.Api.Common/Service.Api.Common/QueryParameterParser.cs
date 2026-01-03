namespace Service.Api.Common;

public class QueryParameterParser : IQueryParameterParser
{
    public (IEnumerable<Guid>, IEnumerable<ValidationError>) ParseGuid(IEnumerable<Guid> originalValues, Dictionary<string, string> queryParameters, string queryParameterKey)
    {
        var guids = new List<Guid>();
        var errors = new List<ValidationError>();

        if (queryParameters.TryGetValue(queryParameterKey, out var ids))
        {
            var values = ids!.Split(",");

            var index = 0;
            foreach (var value in values)
            {
                var isValidGuid = Guid.TryParse(value, out var id);

                if (isValidGuid)
                {
                    guids.Add(id);
                }
                else
                {
                    errors.Add(new()
                    {
                        ErrorCode = ErrorCode.Format,
                        StatusCode = System.Net.HttpStatusCode.UnprocessableContent,
                        Message = $"List query parameter '{queryParameterKey}' has malformed Guid at index {index}.",
                        Location = index.ToString()
                    });
                }

                index++;
            }
        }
        else
        {
            guids = [.. originalValues];
        }

        return (guids, errors);
    }
}
