using CommandLine;
using CommandLine.Text;

namespace WsFederationMetadataTool
{
    internal class Options
    {
        [Option('d', "destination path", Required = true, HelpText = "Metadata file path.")]
        public string MetadataFilePath { get; set; }

        [Option('f', "federation party id", Required = true, HelpText = "Metadata file path.")]
        public string FederationPartyId { get; set; }

        [Option('r', "replace file", Required = false, HelpText = "Metadata file path.")]
        public bool ReplaceFile { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}