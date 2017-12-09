using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace MeshRecovery_Console
{
    class ArgParser
    {
        private OptionSet optionSet;
        private bool showHelp = false;

        public ArgParser()
        {
            optionSet = new OptionSet()
            {
                { "h|help", "show help", v => { ShowHelp(); showHelp = true; } }
            };
        }

        public void AddArgument(string prototype, string description, Action<string> action)
        {
            optionSet.Add(prototype, description, action);
        }

        public bool ParseArguments(IEnumerable<string> args)
        {
            List<string> extras;
            try
            {
                extras = optionSet.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine("Try '--help' for usage information");
                return false;
            }

            if (extras.Count > 0)
            {
                Console.WriteLine($"There are unknown options: {extras.ToString()}");
            }

            if (showHelp)
                return false;

            return true;
        }

        void ShowHelp()
        {
            Console.WriteLine("Usage:");
            foreach (var option in optionSet)
            {
                Console.WriteLine($"    {String.Join(", ", option.GetNames())} - {option.Description};");
            }
        }
    }
}
