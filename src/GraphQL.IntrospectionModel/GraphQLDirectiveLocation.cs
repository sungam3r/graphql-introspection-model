using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel;

/// <summary>
/// Possible locations of directives in the GraphQL schema.
/// This enumeration corresponds to <see cref="DirectiveLocation"/> but has values
/// names matched exactly to ones from spec to simplify JSON deserialization.
/// </summary>
public enum GraphQLDirectiveLocation
{
    // http://spec.graphql.org/October2021/#ExecutableDirectiveLocation

    /// <summary>Location adjacent to a query operation.</summary>
    QUERY,

    /// <summary>Location adjacent to a mutation operation.</summary>
    MUTATION,

    /// <summary>Location adjacent to a subscription operation.</summary>
    SUBSCRIPTION,

    /// <summary>Location adjacent to a field.</summary>
    FIELD,

    /// <summary>Location adjacent to a fragment definition.</summary>
    FRAGMENT_DEFINITION,

    /// <summary>Location adjacent to a fragment spread.</summary>
    FRAGMENT_SPREAD,

    /// <summary>Location adjacent to an inline fragment.</summary>
    INLINE_FRAGMENT,

    /// <summary>Location adjacent to a variable definition.</summary>
    VARIABLE_DEFINITION,

    // http://spec.graphql.org/October2021/#TypeSystemDirectiveLocation

    /// <summary>Location adjacent to a schema definition.</summary>
    SCHEMA,

    /// <summary>Location adjacent to a scalar definition.</summary>
    SCALAR,

    /// <summary>Location adjacent to an object type definition.</summary>
    OBJECT,

    /// <summary>Location adjacent to a field definition.</summary>
    FIELD_DEFINITION,

    /// <summary>Location adjacent to an argument definition.</summary>
    ARGUMENT_DEFINITION,

    /// <summary>Location adjacent to an interface definition.</summary>
    INTERFACE,

    /// <summary>Location adjacent to a union definition.</summary>
    UNION,

    /// <summary>Location adjacent to an enum definition.</summary>
    ENUM,

    /// <summary>Location adjacent to an enum value definition.</summary>
    ENUM_VALUE,

    /// <summary>Location adjacent to an input object type definition.</summary>
    INPUT_OBJECT,

    /// <summary>Location adjacent to an input object field definition.</summary>
    INPUT_FIELD_DEFINITION,
}
