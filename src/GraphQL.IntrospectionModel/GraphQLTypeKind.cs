namespace GraphQL.IntrospectionModel
{
    /// <summary> Kinds of types in the schema. </summary>
    public enum GraphQLTypeKind
    {
        /// <summary> An object, that is, a composite output type. </summary>
        Object,

        /// <summary> Interface. </summary>
        Interface,

        /// <summary> Scalar type. </summary>
        Scalar,

        /// <summary> Enumeration type. </summary>
        Enum,

        /// <summary> A composite input type. </summary>
        Input_Object,

        /// <summary> List of items, type modifier. </summary>
        List,

        /// <summary> A type modifier that prevents the type from being null. </summary>
        Non_Null,

        /// <summary> Union of several types. </summary>
        Union,
    }
}
