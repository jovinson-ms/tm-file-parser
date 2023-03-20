using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TMFileParser.Models.output
{
    [ExcludeFromCodeCoverage]
    public class TM7Diagram
    {
        public string Diagram { get; set; }
        public List<TM7Boundary> Boundaries { get; set; }
        public List<TM7Connector> Connectors { get; set; }
        public List<TM7Asset> Assets { get; set; }
    }
}
