namespace AsyncApiApplicationSupportGenerator
{
    internal class QueryOperationModelStrategy : IOperationModelStrategy
    {
        private readonly string assemblyName;
        private readonly string restMethod;
        private readonly string pathPart;


        public QueryOperationModelStrategy(string assemblyName, string restMethod, string pathPart)
        {
            this.assemblyName = assemblyName;
            this.restMethod = restMethod;
            this.pathPart = pathPart;
        }

        public string Namespace()
        {
            return $"{assemblyName}.Queries";
        }

        public string TypeName()
        {
            return $"{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}Query";
        }
    }
}
