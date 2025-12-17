namespace AsyncApiApplicationSupportGenerator.OperationModelStrategies
{
    internal class CommandResponseOperationModelStrategy : IOperationModelStrategy
    {
        private readonly string assemblyName;
        private readonly string restMethod;
        private readonly string pathPart;


        public CommandResponseOperationModelStrategy(string assemblyName, string restMethod, string pathPart)
        {
            this.assemblyName = assemblyName;
            this.restMethod = restMethod;
            this.pathPart = pathPart;
        }

        public string Namespace()
        {
            return $"{assemblyName}.CommandResponses";
        }

        public string TypeName()
        {
            return $"{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}CommandResponse";
        }
    }
}
