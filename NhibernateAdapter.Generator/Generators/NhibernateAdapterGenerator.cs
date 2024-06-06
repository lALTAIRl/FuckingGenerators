using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NhibernateAdapter.Generator.Templates;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
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
                    string connectionString = "Host=localhost;Port=5432;Database=pgtestdata;Username=postgres;Password=password";
                    var classNames = GetClassNamesFromDatabase(connectionString);

                    var adapterSource = GenerateAdapterSource(classNames);
                    sourceProductionContext.AddSource(
                        $"NhibernateGeneratedAdapter.cs",
                        SourceText.From(adapterSource, Encoding.UTF8));
                });
        }

        private string GenerateAdapterSource(List<string> classNames)
        {
            var mappingString = GenerateMappingString(classNames);
            var typeofsString = GenerateTypeofs(classNames);
            var testDataString = GenerateTestDataString(classNames);
            var testDataPopulatingString = GenerateTestDataPopulatingString(classNames);

            var adapterSource = string.Format(
                AdapterTemplate.Template,
                mappingString,
                typeofsString,
                testDataString,
                testDataPopulatingString);

            return adapterSource;
        }

        private string GenerateMappingString(List<string> classNames)
        {
            var mappingString = new StringBuilder();
            foreach (var className in classNames)
            {
                mappingString.Append("mapper.AddMapping<");
                mappingString.Append(className);
                mappingString.Append("Map>();\n");
            }
            return mappingString.ToString();
        }

        private string GenerateTypeofs(List<string> classNames)
        {
            var typeofsString = new StringBuilder();
            foreach (var className in classNames)
            {
                typeofsString.Append("typeof(");
                typeofsString.Append(className);
                typeofsString.Append("),\n");
            }
            return typeofsString.ToString();
        }

        private string GenerateTestDataString(List<string> classNames)
        {
            var testDataStrtingBuilder = new StringBuilder();
            foreach (var className in classNames)
            {
                testDataStrtingBuilder.Append(
                    $@"var {className.ToLower()}s = new List<{className}>()
            {{
                new {className} {{ Id=1 }},
                new {className} {{ Id=2 }},
                new {className} {{ Id=3 }},
                new {className} {{ Id=4 }},
            }};
            ");
            }
            return testDataStrtingBuilder.ToString();
        }

        private string GenerateTestDataPopulatingString(List<string> classNames)
        {
            var testDataPopulatingString = new StringBuilder();
            foreach (var className in classNames)
            {
                testDataPopulatingString.Append(
                    $@"foreach (var {className.ToLower()} in {className.ToLower()}s)
            {{
                session.SaveOrUpdate({className.ToLower()});
            }}
            ");
            }
            return testDataPopulatingString.ToString();
        }

        private List<string> GetClassNamesFromDatabase(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var db = new QueryFactory(connection, new PostgresCompiler());
            var query = new Query("testclasses").Select("name");

            var classNames = db.Get<string>(query).ToList();

            return classNames;
        }
    }
}
