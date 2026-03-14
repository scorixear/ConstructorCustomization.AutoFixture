using AutoFixture;

using ConstructorCustomization.AutoFixture;

namespace Example.NetStandard21;

public sealed class ExamplePerson
{
    public ExamplePerson(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }

    public string LastName { get; }
}

public static class ExampleUsage
{
    public static ExamplePerson CreatePerson()
    {
        var fixture = new Fixture();

        fixture.Customize(new ConstructorCustomization<ExamplePerson>()
            .With(x => x.FirstName, "Ada")
            .With(x => x.LastName, "Lovelace"));

        return fixture.Create<ExamplePerson>();
    }
}
