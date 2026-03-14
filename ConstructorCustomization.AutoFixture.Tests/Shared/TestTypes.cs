namespace ConstructorCustomization.AutoFixture.Tests.Shared;

internal sealed class CtorParameterCarrier
{
    public CtorParameterCarrier(string firstName, int age)
    {
        FirstName = firstName;
        Age = age;
    }

    public string FirstName { get; }

    public int Age { get; }
}

internal sealed class MultiCtorModel
{
    public MultiCtorModel()
    {
    }

    public MultiCtorModel(string name)
    {
        Name = name;
    }

    public MultiCtorModel(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string? Name { get; }

    public int Age { get; }
}

internal sealed class PersonModel
{
    public PersonModel(string name, int age, string city)
    {
        Name = name;
        Age = age;
        City = city;
    }

    public string Name { get; }

    public int Age { get; }

    public string City { get; }
}

internal sealed class CollectionModel
{
    public CollectionModel(
        int[] scores,
        List<string> names,
        Dictionary<string, int> counts,
        HashSet<Guid> ids)
    {
        Scores = scores;
        Names = names;
        Counts = counts;
        Ids = ids;
    }

    public int[] Scores { get; }

    public List<string> Names { get; }

    public Dictionary<string, int> Counts { get; }

    public HashSet<Guid> Ids { get; }
}

internal sealed class TripleStringModel
{
    public TripleStringModel(string first, string second, string third)
    {
        First = first;
        Second = second;
        Third = third;
    }

    public string First { get; }

    public string Second { get; }

    public string Third { get; }
}
