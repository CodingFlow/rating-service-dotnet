using System;

namespace AsyncApiApplicationSupportGenerator
{
    internal class PostOperationTypeStrategy : IOperationTypeStrategy
    {
        private readonly string assemblyName;
        private readonly string pathPart;

        public PostOperationTypeStrategy(string assemblyName, string pathPart)
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
            return $"{assemblyName}.Commands.Post{StringUtils.ToPascalCase(pathPart)}Command";
        }

        public string ResponseBodyNotPresent()
        {
            return "string";
        }

        public string ResponseBodyPresent()
        {
            return $"{assemblyName}.CommandResponses.Post{StringUtils.ToPascalCase(pathPart)}CommandResponse";
        }
    }
}
