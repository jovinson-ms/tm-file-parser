using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TMFileParser.Models.output
{
    [ExcludeFromCodeCoverage]
    public class TM7All
    {
        public List<TM7Diagram> Diagrams { get; set; }
        public List<TM7Threat> Threats { get; set; }
    }
}
