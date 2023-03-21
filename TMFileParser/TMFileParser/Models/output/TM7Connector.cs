using System.Diagnostics.CodeAnalysis;

namespace TMFileParser.Models.output
{
    [ExcludeFromCodeCoverage]
    public class TM7Connector
    {
        public string Id { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public int HandleX { get; set; }
        public int HandleY { get; set; }

        public int SourceX { get; set; }
        public int SourceY { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
    }
}
