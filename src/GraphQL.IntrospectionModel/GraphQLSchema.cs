using System.Collections.Generic;

namespace GraphQL.IntrospectionModel
{
    /// <summary> Model for GraphQL schema obtained by introspection request. </summary>
    public sealed class GraphQLSchema
    {
        /// <summary> Gets or sets the type used for query. </summary>
        public GraphQLRequestType QueryType { get; set; }

        /// <summary> Gets or sets the type used for mutation. </summary>
        public GraphQLRequestType MutationType { get; set; }

        /// <summary> Gets or sets the type used for subscription. </summary>
        public GraphQLRequestType SubscriptionType { get; set; }

        /// <summary> Gets or sets the types defined in the schema. </summary>
        public ICollection<GraphQLType> Types { get; set; }

        /// <summary> Gets or sets the directives defined in the schema. </summary>
        public ICollection<GraphQLDirective> Directives { get; set; }
    }
}
