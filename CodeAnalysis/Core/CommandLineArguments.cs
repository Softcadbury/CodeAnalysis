namespace CodeAnalysis.Core
{
    using System;

    /// <summary>
    /// Class to manage command line arguments
    /// </summary>
    public static class CommandLineArguments
    {
        /// <summary>
        /// Gets a command line argument based on its name
        /// Example : .\appication.exe -argumentName argumentValue
        /// </summary>
        public static string GetArgument(string argumentName)
        {
            string[] args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length - 1; i++)
            {
                if (args[i] == "-" + argumentName)
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}