using System.Diagnostics.CodeAnalysis;

namespace TMFileParser.Models.output
{
    [ExcludeFromCodeCoverage]
    public class TM7Boundary : IRectangle
    {
        public string Id { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Left { get; set; }
        public decimal Top { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
