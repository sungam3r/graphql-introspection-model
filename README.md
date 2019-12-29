# graphql-introspection-model

[![NuGet](https://img.shields.io/nuget/v/GraphQL.IntrospectionModel)](https://www.nuget.org/packages/GraphQL.IntrospectionModel)
[![Nuget](https://img.shields.io/nuget/dt/GraphQL.IntrospectionModel)](https://www.nuget.org/packages/GraphQL.IntrospectionModel)

![Activity](https://img.shields.io/github/commit-activity/w/sungam3r/graphql-introspection-model)
![Activity](https://img.shields.io/github/commit-activity/m/sungam3r/graphql-introspection-model)
![Activity](https://img.shields.io/github/commit-activity/y/sungam3r/graphql-introspection-model)

![Size](https://img.shields.io/github/repo-size/sungam3r/graphql-introspection-model)

Types for GraphQL [introspection](https://graphql.github.io/graphql-spec/June2018/#sec-Introspection) model. Used by [graphql-sdl-exporter](https://github.com/sungam3r/graphql-sdl-exporter).

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
After deserialization JSON into the `GraphQLSchema` (or after creating `GraphQLSchema` in any other way), it can be printed as SDL document
using [`SDLBuilder`](src/GraphQL.IntrospectionModel/SDL/SDLBuilder.cs):

```c#
GraphQLSchema schema = ...;

// default
var sdl = SDLBuilder.Build(schema);

// customized
var sdl = SDLBuilder.Build(schema, new SDLBuilderOptions { IndentSize = 4, ArgumentComments = false });

File.WriteAllText("MySchema.graphql", sdl);
```

GraphQL has its own language to write GraphQL schemas, [SDL](https://graphql.github.io/graphql-spec/June2018/#sec-Type-System) - Schema Definition Language.
SDL is simple and intuitive to use while being extremely powerful and expressive. Some examples of SDL documents can be found in [graphql-sdl-exporter](https://github.com/sungam3r/graphql-sdl-exporter/tree/master/samples) project.

Many types in this project implement the [`IHasDirectives`](src/GraphQL.IntrospectionModel/IHasDirectives.cs) interface. It serves to obtain information
about the directives applied to the element. The [official specification](https://graphql.github.io/graphql-spec/June2018/#) does not describe such a possibility,
although [discussions](https://github.com/graphql/graphql-spec/issues/300) are underway to expand the specification to add this feature.
