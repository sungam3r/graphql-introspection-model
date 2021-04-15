using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> A field of some input type. </summary>
    [DebuggerDisplay("Input field {Name,nq}")]
    public sealed class GraphQLInputField : GraphQLNamedObject
    {
        /// <summary> Gets or sets the type of this field. </summary>
        public GraphQLFieldType Type { get; set; }

        /// <summary> Gets or sets a GraphQL-formatted string representing the default value for this field. </summary>
        public string DefaultValue { get; set; }
    }
}
