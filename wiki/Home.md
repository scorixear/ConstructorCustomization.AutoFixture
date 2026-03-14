# ConstructorCustomization.AutoFixture Wiki

This wiki is focused on practical usage and extension patterns.

## Start here

1. [Getting Started](Getting-Started)
2. [Customizing Behavior](Customizing-Behavior)
3. [Creation Pipeline](Creation-Pipeline)
4. [Extensions Overview](Extensions-Overview)

## Extension pages

Each page covers one extension point only.

- [Extension: IConstructorSelector](Extension-IConstructorSelector)
- [Extension: IParameterPropertyMatcher](Extension-IParameterPropertyMatcher)
- [Extension: IPropertyExpressionParser](Extension-IPropertyExpressionParser)
- [Extension: IPropertyValueStore](Extension-IPropertyValueStore)
- [Extension: IValueCreationService](Extension-IValueCreationService)
- [Extension: ISpecimenBuilderStrategy](Extension-ISpecimenBuilderStrategy)

## Reading order

1. Start with `With(...)`, `Without(...)`, and `Clear()` from [Getting Started](Getting-Started).
2. Tune default behavior with [Customizing Behavior](Customizing-Behavior).
3. Implement custom extensions only when a real test need appears.

