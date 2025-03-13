using System.Collections.Generic;

namespace D3_Sqex03DataMessage.CLI
{
    public abstract class BaseCommand<TOptions>
    {
        protected List<IValidator<TOptions>> validators;

        public void ExecuteCommand(TOptions options)
        {
            Validate(options);
            Execute(options);
        }

        protected abstract void Execute(TOptions options);

        private void Validate(TOptions options)
        {
            foreach (IValidator<TOptions> validator in validators)
            {
                validator.Validate(options);
            }
        }

    }
}
