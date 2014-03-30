using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestMsDosCommandLineParser
    {
        private MsDosCommandLineParser _parser;
        private MockParserResult _result;
        private List<PositionalArg> _positionals;
        private List<OptionDetails> _options;

        class PositionalArg : IPositionalArgument
        {
            public string ParameterName { get; set; }
        }

        class OptionDetails : IOption
        {
            public string Name { get; set; }
            public bool IsBoolean { get; set; }
            public int ParameterCount { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _positionals = new[] {"pos1", "pos2", "pos3"}.Select(Positional).ToList();
            _options = new[] {"opt", "opt1", "opt2"}.Select(Option).ToList();
            _result = new MockParserResult();
            _parser = new MsDosCommandLineParser();
        }

        [Test]
        public void PositionalParametersAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 param2 p3");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void ParsingStopsWhenPositionalReturnsHalt()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 param2 p3");
            _result.HaltAfter(1);
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void ParsingStopsWhenOptionReturnsHalt()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 /param2 /p3");
            _result.HaltAfter(1);
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void NoParameterOptionsAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("parameter1 /opt");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void OptionNamesAreNotCaseSensitive()
        {
            
            var args = CommandLineTokeniser.Tokenise("/OPt");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void OptionNamesWithParametersAreNotCaseSensitive()
        {
            
            var args = CommandLineTokeniser.Tokenise("/OPt:45");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void UnrecognisedOptionsArePassedThroughVerbatim()
        {
            
            var args = CommandLineTokeniser.Tokenise("/OPt /Unrecognized ");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void ConjoinedOptionParametersAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("/Opt1:arg");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        [Test]
        public void MultipleOptionParametersAreExtracted()
        {
            
            var args = CommandLineTokeniser.Tokenise("/Opt1:arg,45,arg3");
            _parser.Parse(args, _options, _positionals, _result);

            Console.WriteLine(_result.Log);
            Approvals.Verify(_result.Log);
        }

        private PositionalArg Positional(string name)
        {
            return new PositionalArg {ParameterName = name};
        }

        private OptionDetails Option(string name)
        {
            var last = name.Last();
            var num = last > '0' && last <= '9' ? last - '0' : 0;

            return new OptionDetails { Name = name, ParameterCount = num};
        }
    }
}