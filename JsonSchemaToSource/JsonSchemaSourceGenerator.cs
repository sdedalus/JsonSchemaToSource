using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonSchemaToSource
{
    [Generator]
    public class JsonSchemaSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var files = context
                .AdditionalFiles
                .Select(x => x.Path)
                .Where(at => at.EndsWith(".json.schema"))
                .Select(GetJsonSchemaAndName)
                .Select(GetFileContentsAndName);

            foreach (var file in files)
            {
                // inject the created source into the users compilation
                context.AddSource(file.name, SourceText.From(file.file, Encoding.UTF8));
            }
        }

        private static (string file, string name) GetFileContentsAndName((JsonSchema file, string name) schema)
        {
            var fileGenerator = new CSharpGenerator(schema.file);

            var file = fileGenerator
                .GenerateFile(schema.name)
                .Replace("namespace MyNamespace", "namespace JsonClass")
                ;

            return (file, schema.name);
        }

        private static (JsonSchema file, string name) GetJsonSchemaAndName(string path)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
            var file = JsonSchema.FromFileAsync(path).Result;
            return (file, name);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
