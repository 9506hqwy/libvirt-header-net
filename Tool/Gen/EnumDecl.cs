namespace Gen;

internal class EnumDecl
{
    private readonly Dictionary<string, long> values;

    internal EnumDecl(string name)
    {
        this.Name = name;
        this.values = new Dictionary<string, long>();
    }

    internal string Name { get; }

    internal IReadOnlyDictionary<string, long> Values => this.values;

    internal void Add(string name, long value)
    {
        this.values.Add(name, value);
    }
}
