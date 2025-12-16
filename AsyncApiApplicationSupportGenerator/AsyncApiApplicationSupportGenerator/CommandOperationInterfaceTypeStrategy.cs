using System;

namespace AsyncApiApplicationSupportGenerator
{
    internal class CommandOperationInterfaceTypeStrategy : IOperationInterfaceTypeStrategy
    {
        private readonly string assemblyName;
        private readonly string restMethod;
        private readonly string pathPart;

        public CommandOperationInterfaceTypeStrategy(string assemblyName, string restMethod, string pathPart)
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
            return $"{assemblyName}.Commands.{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}Command";
        }

        public string ResponseBodyNotPresent()
        {
            return "string";
        }

        public string ResponseBodyPresent()
        {
            return $"{assemblyName}.CommandResponses.{StringUtils.ToPascalCase(restMethod)}{StringUtils.ToPascalCase(pathPart)}CommandResponse";
        }
    }
}
