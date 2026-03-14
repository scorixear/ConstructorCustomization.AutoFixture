namespace ConstructorCustomization.AutoFixture.Customization.Domain;

internal sealed class CustomizationDomainOptions
{
    private CustomizationDomainOptions(int collectionItemCount, StringComparer propertyNameComparer)
    {
        CollectionItemCount = collectionItemCount;
        PropertyNameComparer = propertyNameComparer;
    }

    public int CollectionItemCount { get; }

    public StringComparer PropertyNameComparer { get; }

    public static CustomizationDomainOptions From(ConstructorCustomizationOptions options)
    {
        ThrowIfNull(options);

        if (options.CollectionItemCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.CollectionItemCount), "CollectionItemCount must be zero or greater.");
        }

        return new CustomizationDomainOptions(options.CollectionItemCount, options.PropertyNameComparer);
    }
}
