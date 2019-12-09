using System;

namespace GraphQL.IntrospectionModel
{
    /// <summary> GraphQL type descriptor. Specifies the kind of the type and its name. </summary>
    public abstract class GraphQLTypeDescriptor
    {
        /// <summary> Gets or sets the type kind. </summary>
        public GraphQLTypeKind Kind { get; set; }

        /// <summary> Gets or sets the type name. </summary>
        public string Name { get; set; }

        /// <summary> Gets a value indicating whether the type is introspection type. </summary>
        public bool IsIntrospection => Name?.StartsWith("__", StringComparison.Ordinal) == true;
    }
}
