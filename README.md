# graphql-introspection-model

<a href="https://www.buymeacoffee.com/sungam3r" target="_blank"><img src="https://bmc-cdn.nyc3.digitaloceanspaces.com/BMC-button-images/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

![License](https://img.shields.io/github/license/sungam3r/graphql-introspection-model)

[![codecov](https://codecov.io/gh/sungam3r/graphql-introspection-model/branch/master/graph/badge.svg?token=I98DQMW5EJ)](https://codecov.io/gh/sungam3r/graphql-introspection-model)
[![Nuget](https://img.shields.io/nuget/dt/GraphQL.IntrospectionModel)](https://www.nuget.org/packages/GraphQL.IntrospectionModel)
[![NuGet](https://img.shields.io/nuget/v/GraphQL.IntrospectionModel)](https://www.nuget.org/packages/GraphQL.IntrospectionModel)

[![GitHub Release Date](https://img.shields.io/github/release-date/sungam3r/graphql-introspection-model?label=released)](https://github.com/sungam3r/graphql-introspection-model/releases)
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/sungam3r/graphql-introspection-model/latest?label=new+commits)](https://github.com/sungam3r/graphql-introspection-model/commits/master)
![Size](https://img.shields.io/github/repo-size/sungam3r/graphql-introspection-model)

[![GitHub contributors](https://img.shields.io/github/contributors/sungam3r/graphql-introspection-model)](https://github.com/sungam3r/graphql-introspection-model/graphs/contributors)
![Activity](https://img.shields.io/github/commit-activity/w/sungam3r/graphql-introspection-model)
![Activity](https://img.shields.io/github/commit-activity/m/sungam3r/graphql-introspection-model)
![Activity](https://img.shields.io/github/commit-activity/y/sungam3r/graphql-introspection-model)

[![Run unit tests](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/test.yml/badge.svg)](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/test.yml)
[![Publish preview to GitHub registry](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/publish-preview.yml/badge.svg)](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/publish-preview.yml)
[![Publish release to Nuget registry](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/publish-release.yml/badge.svg)](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/publish-release.yml)
[![CodeQL analysis](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/sungam3r/graphql-introspection-model/actions/workflows/codeql-analysis.yml)

Types for GraphQL [introspection](https://graphql.github.io/graphql-spec/October2021/#sec-Introspection) model. Used by [graphql-sdl-exporter](https://github.com/sungam3r/graphql-sdl-exporter).

A GraphQL server supports introspection over its schema. This schema is queried using GraphQL itself, creating a powerful
platform for tool‐building.

Here's what a "classic" introspection query looks like:
```graphql
query IntrospectionQuery {
  __schema {
    queryType { name }
    mutationType { name }
    subscriptionType { name }
    types {
      ...FullType
    }
    directives {
      name
      description
      locations
      args {
        ...InputValue
      }
    }
  }
}

fragment FullType on __Type {
  kind
  name
  description
  fields(includeDeprecated: true) {
    name
    description
    args {
      ...InputValue
    }
    type {
      ...TypeRef
    }
    isDeprecated
    deprecationReason
  }
  inputFields {
    ...InputValue
  }
  interfaces {
    ...TypeRef
  }
  enumValues(includeDeprecated: true) {
    name
    description
    isDeprecated
    deprecationReason
  }
  possibleTypes {
    ...TypeRef
  }
}

fragment InputValue on __InputValue {
  name
  description
  type { ...TypeRef }
  defaultValue
}

fragment TypeRef on __Type {
  kind
  name
  ofType {
    kind
    name
    ofType {
      kind
      name
      ofType {
        kind
        name
        ofType {
          kind
          name
          ofType {
            kind
            name
            ofType {
              kind
              name
              ofType {
                kind
                name
              }
            }
          }
        }
      }
    }
  }
}
```

The result of this query (like all other GraphQL queries) is JSON. You can deal with it directly or deserialize it into some data structures.
Such data structures are provided by this repository. The top level type is [`GraphQLSchema`](src/GraphQL.IntrospectionModel/GraphQLSchema.cs).
After deserialization JSON into the `GraphQLSchema` (or after creating `GraphQLSchema` in any other way), it can be transformed into AST
representation and then printed by `SDLPrinter` from [GraphQL-Parser](https://github.com/graphql-dotnet/parser) nuget package.

#### Example of deserializing introspection response

```csharp
using System.Text.Json;

string text = ...; // from HTTP introspection response
var response = JsonSerializer.Deserialize<GraphQLResponse>(text, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    Converters = { new JsonStringEnumConverter() }
});
var schema = response.Data.__Schema; // note that Data may be null, so check response.Errors 
```

#### Example of printing `GraphQLSchema` into SDL

```csharp
GraphQLSchema schema = ...;

var converter = new ASTConverter();
var document = converter.ToDocument(schema);
var printer = new SDLPrinter(options);
var sdl = printer.Print(document);

// or use one-line extension method
var sdl = schema.Print();
```

GraphQL has its own language to write GraphQL schemas, [SDL](https://graphql.github.io/graphql-spec/October2021/#sec-Type-System) - Schema Definition Language.
SDL is simple and intuitive to use while being extremely powerful and expressive. Some examples of SDL documents can be found in [graphql-sdl-exporter](https://github.com/sungam3r/graphql-sdl-exporter/tree/master/samples) project.

Many types in this project implement the [`IHasDirectives`](src/GraphQL.IntrospectionModel/IHasDirectives.cs) interface. It serves to obtain information
about the directives applied to the element. The [official specification](https://graphql.github.io/graphql-spec/October2021) does not describe such a possibility,
although [discussions](https://github.com/graphql/graphql-spec/issues/300) are underway to expand the specification to add this feature.
[graphql-sdl-exporter](https://github.com/sungam3r/graphql-sdl-exporter/tree/master/samples) can get information about directives if the server
supports this feature.
