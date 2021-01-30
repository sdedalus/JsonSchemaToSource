using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSchemaToSource
{
    [Generator]
    public class JsonSchemaSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace))
            {
                return;
            }

            string GetNamespace(AdditionalText file)
            {
                //Contract.Assert(false);
                if (context.AnalyzerConfigOptions.GetOptions(file).TryGetValue("build_metadata.AdditionalFiles.Namespace", out var @namespace)
                 && !string.IsNullOrWhiteSpace(@namespace))
                    return @namespace;
                else
                    return rootNamespace;
            }

            var files = from file in context.AdditionalFiles
                        let path = file.Path
                        where path.EndsWith(".json.schema")
                        let @namespace = GetNamespace(file)
                        select GenerateSourceFromSchemaFileAsync(path, @namespace);

            var tasks = files.ToList();
            while (tasks.Any())
            {
                var tasksArray = tasks.ToArray();
                var completedFileIndex = Task.WaitAny(tasksArray);
                var completedFileTask = tasksArray[completedFileIndex];
                var file = completedFileTask.Result;
                context.AddSource(file.name, SourceText.From(file.file, Encoding.UTF8));
                tasks.Remove(completedFileTask);
            }
        }

        private static async Task<(string name, string file)> GenerateSourceFromSchemaFileAsync(string path, string @namespace)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
            var schema = await JsonSchema.FromFileAsync(path);
            var settings = new CSharpGeneratorSettings { Namespace = @namespace };
            var fileGenerator = new CSharpGenerator(schema, settings);
            string file = fileGenerator.GenerateFile(name);
            return (name, file);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
