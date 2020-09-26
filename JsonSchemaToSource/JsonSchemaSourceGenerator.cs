using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace JsonSchemaToSource
{
    public class JsonSchemaSourceGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
        }

        public void Initialize(InitializationContext context)
        {
            // No initializationRequired
        }
    }
}
