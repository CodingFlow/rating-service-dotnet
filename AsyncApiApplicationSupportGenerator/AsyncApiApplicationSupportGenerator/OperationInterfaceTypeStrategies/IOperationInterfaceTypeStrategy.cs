namespace AsyncApiApplicationSupportGenerator.OperationInterfaceTypeStrategies
{
    internal interface IOperationInterfaceTypeStrategy
    {
        string RequestBodyPresent();
        string RequestBodyNotPresent();
        string ResponseBodyPresent();
        string ResponseBodyNotPresent();
    }
}