namespace SourceGenerator.Models
{
    internal class GeneratedSource
    {
        public string Name { get; set; }
        public string Source { get; set; }

        public GeneratedSource(string name, string source)
        {
            this.Name = name;
            this.Source = source;
        }
    }
}
