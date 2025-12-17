namespace AsyncApiApplicationSupportGenerator.OperationInterfaceTypeStrategies
{
    internal class QueryOperationInterfaceTypeStrategy : IOperationInterfaceTypeStrategy
    {
        private readonly string assemblyName;
        private readonly string restMethod;
        private readonly string pathPart;

        public QueryOperationInterfaceTypeStrategy(string assemblyName, string restMethod, string pathPart)
        {
            this.assemblyName = assemblyName;
            this.restMethod = restMethod;
            this.pathPart = pathPart;
        }

        public string RequestBodyNotPresent()
        {
            return string.Empty;
        }

        public string RequestBodyPresent()
        {
            return $"{assemblyName}.Queries.{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}Query";
        }

        public string ResponseBodyNotPresent()
        {
            return string.Empty;
        }

        public string ResponseBodyPresent()
        {
            return $"{assemblyName}.QueryResponses.{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}QueryResponse";
        }
    }
}
