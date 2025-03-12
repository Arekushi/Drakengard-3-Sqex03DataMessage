namespace D3_Sqex03DataMessage.CLI
{
    public interface IValidator<T>
    {
        void Validate(T options);
    }
}
