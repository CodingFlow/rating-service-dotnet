namespace AsyncApiApplicationSupportGenerator
{
    internal interface IOperationInterfaceTypeStrategy
    {
        string RequestBodyPresent();
        string RequestBodyNotPresent();
        string ResponseBodyPresent();
        string ResponseBodyNotPresent();
    }
}