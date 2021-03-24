using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary>
    /// Argument of the directive applied to some element in the schema.
    /// </summary>
    /// <remarks>
    /// This is an experimental type, the official specification does not contain such a concept.
    /// </remarks>
    [DebuggerDisplay("Argument '{Name,nq}'={Value}")]
    public sealed class GraphQLDirectiveArgument
    {
        /// <summary> Gets or sets the name of the argument. </summary>
        public string Name { get; set; }

        /// <summary> Gets or sets the value of the argument. </summary>
        public string Value { get; set; }
    }
}
