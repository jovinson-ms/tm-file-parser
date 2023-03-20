using System;
using System.Diagnostics.CodeAnalysis;

namespace TMFileParser.Models.output
{
    [ExcludeFromCodeCoverage]
    public class TM7Threat
    {
        public string Id { get; set; }
        public string Diagram { get; set; }
        public string ChangedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Justifications { get; set; }
        public string Interaction { get; set; }
        public string Priority { get; set; }
    }
}
