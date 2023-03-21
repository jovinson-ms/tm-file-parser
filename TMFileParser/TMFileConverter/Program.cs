using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TMFileParser;

namespace TMFileConverter
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();
            var inputPathOption = new Option<FileInfo>(
                    "--input-path",
                    "Input tm7 file path.");
            inputPathOption.AddAlias("-i");
            inputPathOption.Required = true;

            var formatOption = new Option<string>(
                    "--save-format",
                    "Output file format to convert.");
            formatOption.AddAlias("-s");
            formatOption.Required = true;

            var actionOption = new Option<string[]>(
                    "--get",
                    "Data to be retrieved.");
            actionOption.AddAlias("-g");
            actionOption.Required = true;

            var outputPathOption = new Option<FileInfo>(
                    "--output-path",
                    "Path to store output.");
            outputPathOption.AddAlias("-o");
            outputPathOption.Required = true;

            rootCommand.AddOption(inputPathOption);
            rootCommand.AddOption(formatOption);
            rootCommand.AddOption(outputPathOption);
            rootCommand.AddOption(actionOption);
            rootCommand.Description = "Command line tool to parse tm7 and tb7 files.";
            rootCommand.Handler = CommandHandler.Create<FileInfo, string, string[], FileInfo>(RunCommand);
            await rootCommand.InvokeAsync(args);
        }

        public static void PrintError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        public static void RunCommand(FileInfo inputpath, string saveformat, string[] get, FileInfo outputpath)
        {
            try
            {
                if (inputpath == null || saveformat == null || get == null || outputpath == null)
                {
                    throw new ArgumentNullException("Missing inputs. -i/--input-path, -g/--get, -o/--output-path and -s/--save-format are required.");
                }
                var extension = inputpath.Extension.ToLower();
                if (extension != ".tm7" && extension != ".tb7")
                {
                    throw new ArgumentException("Invalid -i/--input-path.");
                }
                var parser = new Parser(inputpath);
                foreach (string category in get)
                {
                    object result = parser.GetData(category);
                    string output, outputFilePath;
                    switch (saveformat)
                    {
                        case "json":
                            output = JsonSerializer.Serialize(result);
                            outputFilePath = Path.Combine(outputpath.FullName, Path.GetFileNameWithoutExtension(inputpath.FullName) + "-" + category + ".json");
                            SaveToFile(output, outputFilePath);
                            break;
                        case "mermaid":
                            output = MermaidMarkdownConverter.Convert(result);
                            outputFilePath = Path.Combine(outputpath.FullName, Path.GetFileNameWithoutExtension(inputpath.FullName) + "-" + category + ".md");
                            SaveToFile(output, outputFilePath);
                            break;
                        case "png":
                            outputFilePath = Path.Combine(outputpath.FullName, Path.GetFileNameWithoutExtension(inputpath.FullName));
                            ImageRenderer.RenderImage(result, outputFilePath);
                            break;
                        default:
                            throw new ArgumentException("Invalid -s/--save-format:" + saveformat);
                    }

                    
                }
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }    
        }

        private static void SaveToFile(string output, string outputFilePath)
        {
            try
            {
                File.WriteAllText(outputFilePath, output);
            }
            catch
            {
                throw new IOException("Error occured while converting the file.");
            }

            Console.WriteLine();
            Console.WriteLine("File saved. Path : " + outputFilePath);
        }
                  
    }
}
