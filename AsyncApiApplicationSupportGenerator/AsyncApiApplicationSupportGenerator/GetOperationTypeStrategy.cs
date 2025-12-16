using System;

namespace AsyncApiApplicationSupportGenerator
{
    internal class GetOperationTypeStrategy : IOperationTypeStrategy
    {
        private readonly string assemblyName;
        private readonly string pathPart;

        public GetOperationTypeStrategy(string assemblyName, string pathPart)
        {
            this.assemblyName = assemblyName;
            this.pathPart = pathPart;
        }

        public string RequestBodyNotPresent()
        {
            return string.Empty;
        }

        public string RequestBodyPresent()
        {
            return $"{assemblyName}.Queries.Get{StringUtils.ToPascalCase(pathPart)}Query";
        }

        public string ResponseBodyNotPresent()
        {
            return string.Empty;
        }

        public string ResponseBodyPresent()
        {
            return $"{assemblyName}.QueryResponses.Get{StringUtils.ToPascalCase(pathPart)}QueryResponse";
        }
    }
}
