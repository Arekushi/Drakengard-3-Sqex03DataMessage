using D3_Sqex03DataMessage.CLI;
using System;

namespace D3_Sqex03DataMessage
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.Execute(args);
        }
    }
}
