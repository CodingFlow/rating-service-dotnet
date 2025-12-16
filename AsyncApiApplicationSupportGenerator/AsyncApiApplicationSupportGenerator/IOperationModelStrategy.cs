namespace AsyncApiApplicationSupportGenerator
{
    internal interface IOperationModelStrategy
    {
        string Namespace();
        string TypeName();
    }
}