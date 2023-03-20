using System.Net;
using System.Runtime.CompilerServices;

namespace TMFileParser.Models.output
{
    public interface IRectangle
    {
        decimal Left { get; set; }
        decimal Top { get; set; }
        decimal Width { get; set; }
        decimal Height { get; set; }
    }

    public static class IRectangleExtensions
    {
        public static bool Contains(this IRectangle self, IRectangle candidate)
        {
            return (self.Left < candidate.Left &&
                self.Top < candidate.Top &&
                self.Left + self.Width > candidate.Left + candidate.Width &&
                self.Top + self.Height > candidate.Top + candidate.Height);
        }
    }
}
