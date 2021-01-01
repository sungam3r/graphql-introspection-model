using System.Collections.Generic;
using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> A field of some output type. </summary>
    [DebuggerDisplay("{Name,nq}: {Type.SDLType,nq}")]
    public sealed class GraphQLField : GraphQLNamedObject, IDeprecatable
    {
        /// <summary> Gets or sets the type of this field. </summary>
        public GraphQLFieldType Type { get; set; }

        /// <summary> Gets or sets the list of field arguments. </summary>
        public ICollection<GraphQLArgument> Args { get; set; }

        /// <summary> Gets or sets a value indicating whether this field is deprecated. </summary>
        public bool IsDeprecated { get; set; }

        /// <summary> Gets or sets the reason for why field is deprecated. </summary>
        public string DeprecationReason { get; set; }
    }
}
