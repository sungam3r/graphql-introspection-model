using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> An argument to a field or directive. </summary>
    [DebuggerDisplay("Argument '{Name,nq}'")]
    public sealed class GraphQLArgument : GraphQLNamedObject
    {
        /// <summary> Gets or sets the type of this argument. </summary>
        public GraphQLFieldType Type { get; set; }

        /// <summary> Gets or sets the default argument value. </summary>
        public object DefaultValue { get; set; }
    }
}
