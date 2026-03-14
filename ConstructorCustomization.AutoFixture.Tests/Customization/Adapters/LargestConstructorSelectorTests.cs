using ConstructorCustomization.AutoFixture.Tests.Shared;

namespace ConstructorCustomization.AutoFixture.Tests.Customization.Adapters;

[TestFixture]
internal sealed class LargestConstructorSelectorTests
{
    [Test]
    public void SelectConstructor_WhenConstructorsExist_ReturnsLargestByArity()
    {
        var selector = new LargestConstructorSelector();
        var constructors = typeof(MultiCtorModel).GetConstructors();

        var selected = selector.SelectConstructor(typeof(MultiCtorModel), constructors);

        Assert.That(selected.GetParameters().Length, Is.EqualTo(2));
    }

    [Test]
    public void SelectConstructor_WhenNoConstructors_ThrowsInvalidOperationException()
    {
        var selector = new LargestConstructorSelector();

        Assert.That(
            () => selector.SelectConstructor(typeof(string), []),
            Throws.InvalidOperationException.With.Message.Contains("does not have any public constructors"));
    }

    [Test]
    public void SelectConstructor_WhenArgumentsAreNull_ThrowsArgumentNullException()
    {
        var selector = new LargestConstructorSelector();
        var constructors = typeof(MultiCtorModel).GetConstructors();

        Assert.Multiple(() =>
        {
            Assert.That(() => selector.SelectConstructor(null!, constructors), Throws.ArgumentNullException);
            Assert.That(() => selector.SelectConstructor(typeof(MultiCtorModel), null!), Throws.ArgumentNullException);
        });
    }
}
