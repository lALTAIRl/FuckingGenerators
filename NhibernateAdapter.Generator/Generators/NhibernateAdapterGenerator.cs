using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NhibernateAdapter.Generator.Templates;
using System.Collections.Generic;
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
                    //GET THEM FROM SOMEWHERE -> DATAMODEL
                    var classNames = new List<string>() { "Animal", "Application", "Building", "Image"};

                    var adapterSource = GenerateAdapterSource(classNames);
                    sourceProductionContext.AddSource(
                        $"NhibernateAdapter.cs",
                        SourceText.From(adapterSource, Encoding.UTF8));
                });
        }

        private string GenerateAdapterSource(List<string> classNames)
        {
            var mappringString = GenerateMappingString(classNames);
            var typeofsString = GenerateTypeofs(classNames);
            var testDataString = GenerateTestDataString(classNames);
            var testDataPopulatingString = GenerateTestDataPopulatingString(classNames);

            var adapterSource = string.Format(
                AdapterTemplate.Template,
                mappringString,
                typeofsString,
                testDataString,
                testDataPopulatingString);

            return adapterSource;
        }

        private string GenerateMappingString(List<string> classNames)
        {
            //{0}

            //mapper.AddMapping<AnimalMap>();
            //mapper.AddMapping<ApplicationMap>();
            //mapper.AddMapping<BuildingMap>();
            //mapper.AddMapping<ImageMap>();

            var mappingString = new StringBuilder();
            foreach (var className in classNames)
            {
                mappingString.Append($@"mapper.AddMapping<{className}Map>();
            ");
            }
            return mappingString.ToString();
        }

        private string GenerateTypeofs(List<string> classNames)
        {
            //{1}

            //typeof(Animal),
            //typeof(Application),
            //typeof(Building),
            //typeof(Image),

            var typeofsString = new StringBuilder();
            foreach (var className in classNames)
            {
                typeofsString.Append($@"typeof({className}),
                ");
            }
            return typeofsString.ToString();
        }

        private string GenerateTestDataString(List<string> classNames)
        {
            //{2}

            //var animals = new List<Animal>()
            //{{
            //    new Animal {{ Id=1 }},
            //    new Animal {{ Id=2 }},
            //    new Animal {{ Id=3 }},
            //    new Animal {{ Id=4 }},
            //}};

            //var applications = new List<Application>()
            //{{
            //    new Application {{ Id=1 }},
            //    new Application {{ Id=2 }},
            //}};

            //var buildings = new List<Building>()
            //{{
            //    new Building {{ Id=1 }},
            //}};

            //var images = new List<Image>()
            //{{
            //    new Image {{ Id=1 }},
            //    new Image {{ Id=2 }},
            //}};

            var testDataString = new StringBuilder();
            foreach (var className in classNames)
            {
                testDataString.Append(
                    $@"var {className.ToLower()} = new List<{className}>()
            {{
                new {className} {{ Id=1 }},
                new {className} {{ Id=2 }},
                new {className} {{ Id=3 }},
                new {className} {{ Id=4 }},
            }};
            ");
            }
            return testDataString.ToString();
        }

        private string GenerateTestDataPopulatingString(List<string> classNames)
        {
            //{3}

            //foreach (var animal in animals)
            //{
            //    {
            //        session.SaveOrUpdate(animal);
            //    }
            //}
            //
            //foreach (var application in applications)
            //{
            //    {
            //        session.SaveOrUpdate(application);
            //    }
            //}
            //
            //foreach (var building in buildings)
            //{
            //    {
            //        session.SaveOrUpdate(building);
            //    }
            //}
            //
            //foreach (var image in images)
            //{
            //    {
            //        session.SaveOrUpdate(image);
            //    }
            //}

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
    }
}
