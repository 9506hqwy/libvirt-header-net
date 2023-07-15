namespace Gen;

internal class EnumDecl
{
    private readonly Dictionary<string, int> values;

    internal EnumDecl(string name)
    {
        this.Name = name;
        this.values = new Dictionary<string, int>();
    }

    internal string Name { get; }

    internal IReadOnlyDictionary<string, int> Values => this.values;

    internal void Add(string name, int value)
    {
        this.values.Add(name, value);
    }
}
