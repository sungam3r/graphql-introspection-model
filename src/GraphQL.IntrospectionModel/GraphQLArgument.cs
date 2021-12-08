using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> An argument to a field or directive. </summary>
    [DebuggerDisplay("Argument '{Name,nq}'")]
    public sealed class GraphQLArgument : GraphQLNamedObject
    {
        /// <summary> Gets or sets the type of this argument. </summary>
        public GraphQLFieldType Type { get; set; } = null!;

        /// <summary> Gets or sets a GraphQL-formatted string representing the default value for this argument. </summary>
        public string? DefaultValue { get; set; }
    }
}
