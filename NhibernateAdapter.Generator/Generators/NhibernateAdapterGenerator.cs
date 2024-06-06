using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace NhibernateAdapter.Generator.Generators
{
    [Generator]
    public class NhibernateAdapterGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.AnalyzerConfigOptionsProvider,
                (sourceProductionContext, optionsProvider) =>
                {
                    //string connectionString = "Host=localhost;Port=5432;Database=pgtestdata;Username=postgres;Password=password";
                    //var classNames = GetClassNamesFromDatabase(connectionString);

                    //foreach (var className in classNames)
                    //{
                    //    var classSource = GenerateClassSource(className);
                    //    sourceProductionContext.AddSource(
                    //        $"{className}.cs",
                    //        SourceText.From(classSource, Encoding.UTF8));

                    //    var mappingSource = GenerateMappingSource(className);
                    //    sourceProductionContext.AddSource(
                    //        $"{className}Map.cs",
                    //        SourceText.From(mappingSource, Encoding.UTF8));
                    //}
                });
        }
    }
}
