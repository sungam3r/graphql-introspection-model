using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> The type of the GraphQL request: query / mutation / subscription. </summary>
    [DebuggerDisplay("{Name,nq}")]
    public sealed class GraphQLRequestType
    {
        /// <summary> Gets or sets the type name of the GraphQL request: query/mutation/subscription. </summary>
        public string Name { get; set; }
    }
}
