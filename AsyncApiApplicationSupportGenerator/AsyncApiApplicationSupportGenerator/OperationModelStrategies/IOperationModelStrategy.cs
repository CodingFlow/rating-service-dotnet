namespace AsyncApiApplicationSupportGenerator.OperationModelStrategies
{
    internal interface IOperationModelStrategy
    {
        string Namespace();
        string TypeName();
    }
}