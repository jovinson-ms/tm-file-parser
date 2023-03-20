using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMFileParser.Models.output;

namespace TMFileConverter
{
    /// <summary>
    /// Creates a Mermaid Markdown file for each diagram.
    /// </summary>
    internal static class MermaidMarkdownConverter
    {
        public static string Convert(object model)
        {
            var data = model as TM7All;
            
            var result = new StringBuilder();

            foreach (var diagram in data.diagrams)
            {
                result.AppendLine($"# {diagram.diagram}");
                result.AppendLine(":::mermaid");
                result = PrintDiagram(result, diagram);
                result.AppendLine(":::");
            }

            return result.ToString();
        }

        private static StringBuilder PrintDiagram(StringBuilder result, TM7Diagram diagram)
        {
            result.AppendLine("flowchart LR");

            var outerBoundary = new TM7Boundary { Id = "root", DisplayName = diagram.diagram, Top = 0, Left = 0, Height = 10000, Width = 10000 };
            var rootLevel = new NestedLevel(outerBoundary);
            var levels = diagram.boundaries.Select(x => new NestedLevel(x)).ToDictionary(x => x.Id);
            levels.Add(rootLevel.Id, rootLevel);

            foreach (var asset in diagram.assets)
            {
                var parent = levels.Where(x => x.Value.Contains(asset)).OrderBy(x => x.Value.Height * x.Value.Width).FirstOrDefault();

                levels[parent.Value.Id].Assets.Add(asset);
            }

            foreach (var (levelId, level) in levels)
            {
                var parent = levels.Where(x => x.Value.Contains(level)).OrderBy(x => x.Value.Height * x.Value.Width).FirstOrDefault();
                if (parent.Key is null)
                {
                    continue;
                }
                levels[parent.Value.Id].Levels.Add(level);
            }

            var subgraphs = levels[rootLevel.Id];

            result = PrintSubgraphs(result, subgraphs);

            foreach (var connector in diagram.connectors)
            {
                result.AppendLine($"{connector.SourceId}-. \"{connector.DisplayName}\" .-> {connector.TargetId}");
            }

            return result;
        }

        private static StringBuilder PrintSubgraphs(StringBuilder result, NestedLevel currentLevel)
        {
            result.AppendLine($"subgraph {currentLevel.Id} [\"{currentLevel.DisplayName}\"]");

            foreach (var asset in currentLevel.Assets)
            {
            }

            foreach (var subLevel in currentLevel.Levels)
            {
                PrintSubgraphs(result, subLevel);
            }

            result.AppendLine("end");
            return result;
        }
    }

    internal class NestedLevel : TM7Boundary
    {
        public List<TM7Asset> Assets = new();
        public List<NestedLevel> Levels = new();

        public NestedLevel()
        {
        }

        public NestedLevel(TM7Boundary Boundary)
        {
            Id = Boundary.Id;
            Top = Boundary.Top;
            Left = Boundary.Left;
            Height = Boundary.Height;
            Width = Boundary.Width;
            Name = Boundary.Name;
            DisplayName = Boundary.DisplayName;
        }
    }
}
