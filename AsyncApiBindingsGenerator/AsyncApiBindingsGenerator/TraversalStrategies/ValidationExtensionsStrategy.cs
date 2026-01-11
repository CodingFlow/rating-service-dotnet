using System;
using System.Collections.Generic;
using System.Linq;
using AsyncApiBindingsGenerator.Utils;
using ByteBard.AsyncAPI.Models;

namespace AsyncApiBindingsGenerator.TraversalStrategies
{
    internal class ValidationExtensionsStrategy : ITraversalStrategy<ValidationExtensionsInput, ValidationExtensionsInput>
    {
        public ValidationExtensionsInput ProcessParent(ValidationExtensionsInput input, AsyncApiDocument spec)
        {
            return input;
        }

        public ValidationExtensionsInput ProcessChild(KeyValuePair<string, AsyncApiJsonSchema> propertyEntry, ValidationExtensionsInput parentInput, AsyncApiDocument spec)
        {
            var propertyName = propertyEntry.Key;
            var propertyValue = propertyEntry.Value;

            var childSchema = propertyValue.Type == SchemaType.Array
                ? propertyValue.Items
                : propertyValue;

            var childValidatedType = propertyValue.Type == SchemaType.Array
                ? StringUtils.ToPascalCase(spec.Components.Schemas.First(entry => propertyValue.Items.Equals(entry.Value.Schema)).Key)
                : StringUtils.ToPascalCase(propertyName);

            var childValidatorName = $"{childValidatedType}Validator";

            return new ValidationExtensionsInput
            {
                Schema = childSchema,
                ValidatedType = childValidatedType,
                ValidatorName = childValidatorName
            };
        }
    }
}
