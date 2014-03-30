﻿using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public class MsDosCommandLineParser : ICommandLineParser
    {
        public void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
        {
            var optionNames = options.Select(o => o.Name).ToList();
            foreach (var arg in args)
            {
                ParseOutcome outcome;
                if (arg.StartsWith("/"))
                    outcome = ProcessOption(optionNames, arg, result);
                else
                    outcome = result.PositionalArgument(arg);

                if (outcome == ParseOutcome.Halt)
                    return;
            }
        }

        private ParseOutcome ProcessOption(List<string> optionNames, string arg, IParserResult result)
        {
            var colonPos = arg.IndexOf(':');
            if (colonPos < 0)
            {
                var optionName = arg.Substring(1);
                return result.OptionExtracted(GetOptionName(optionNames, optionName), new string[] {});
            }

            var option = arg.Substring(1, colonPos - 1);
            var optionArgs = arg.Substring(colonPos + 1).Split(',');
            return result.OptionExtracted(GetOptionName(optionNames, option), optionArgs);
        }

        private static string GetOptionName(List<string> optionNames, string optionName)
        {
            return optionNames.FirstOrDefault(n => string.Compare(optionName, n, true) == 0) ?? optionName;
        }
    }
}