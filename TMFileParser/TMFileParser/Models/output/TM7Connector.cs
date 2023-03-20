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
    }
}
