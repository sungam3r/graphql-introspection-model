using System.Collections.Generic;
using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary>
    /// The directive defined by the schema. The application of this directive to a schema element
    /// is described through <see cref="GraphQLDirectiveUsage"/>.
    /// </summary>
    [DebuggerDisplay("Directive '{Name,nq}'")]
    public sealed class GraphQLDirective : GraphQLNamedObject
    {
        /// <summary> Gets or sets the possible locations of this directive in the schema. </summary>
        public ICollection<GraphQLDirectiveLocation> Locations { get; set; }

        /// <summary> Gets or sets the argument list of the directive. </summary>
        public ICollection<GraphQLArgument> Args { get; set; }
    }
}
