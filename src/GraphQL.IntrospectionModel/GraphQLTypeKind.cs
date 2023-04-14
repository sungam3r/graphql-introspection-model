namespace GraphQL.IntrospectionModel;

/// <summary> Kinds of types in the schema. </summary>
public enum GraphQLTypeKind
{
    /// <summary> An object, that is, a composite output type. </summary>
    OBJECT,

    /// <summary> Interface. </summary>
    INTERFACE,

    /// <summary> Scalar type. </summary>
    SCALAR,

    /// <summary> Enumeration type. </summary>
    ENUM,

    /// <summary> A composite input type. </summary>
    INPUT_OBJECT,

    /// <summary> List of items, type modifier. </summary>
    LIST,

    /// <summary> A type modifier that prevents the type from being null. </summary>
    NON_NULL,

    /// <summary> Union of several types. </summary>
    UNION,
}
