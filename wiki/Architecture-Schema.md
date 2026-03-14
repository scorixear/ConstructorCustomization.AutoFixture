# Architecture Schema

The package is organized using Hexagonal Architecture (Ports and Adapters), grouped by business capability.

## Capabilities

- Customization: constructor orchestration, parameter-to-property mapping, and configured-value precedence.
- Value generation: plugin and value-service pipeline.
- Specimen generation: collection/type-shape builders.

## Schema types and dependency rules

- Domain: business values and policies.
  - Can use: Domain only.
- Application: orchestration and contracts used by the public API.
  - Can use: Domain and ports.
- Adapters: default implementations for contracts.
  - Can use: Application and Domain ports.
- Public API: entry points exposed to consumers.
  - Can use: Application abstractions and composition only.

## Public versus internal contracts

Public extension ports:

- IValueCreationPlugin
- ISpecimenBuilderStrategy
- IValueCreationService
- IConstructorSelector
- IParameterPropertyMatcher
- IPropertyExpressionParser
- IPropertyValueStore

These contracts are consumed through protected `Use...` hooks on ConstructorCustomization<T, TSelf>.

## Value precedence rule

Configured values are resolved in this order:

1. Override values (With/Without)
2. Default values (SetDefault)
3. Generated values (value service pipeline)
