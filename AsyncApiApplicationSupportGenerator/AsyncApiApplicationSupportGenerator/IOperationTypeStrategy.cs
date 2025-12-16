namespace AsyncApiApplicationSupportGenerator
{
    internal interface IOperationTypeStrategy
    {
        string RequestBodyPresent();
        string RequestBodyNotPresent();
        string ResponseBodyPresent();
        string ResponseBodyNotPresent();
    }
}