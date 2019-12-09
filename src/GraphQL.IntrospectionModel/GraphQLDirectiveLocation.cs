#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GraphQL.IntrospectionModel
{
    /// <summary> Possible locations of directives in the GraphQL schema. </summary>
    public enum GraphQLDirectiveLocation
    {
        // ExecutableDirectiveLocation
        Query,
        Mutation,
        Subscription,
        Field,
        Fragment_Definition,
        Fragment_Spread,
        Inline_Fragment,

        // TypeSystemDirectiveLocation
        Schema,
        Scalar,
        Object,
        Field_Definition,
        Argument_Definition,
        Interface,
        Union,
        Enum,
        Enum_Value,
        Input_Object,
        Input_Field_Definition,
    }
}
