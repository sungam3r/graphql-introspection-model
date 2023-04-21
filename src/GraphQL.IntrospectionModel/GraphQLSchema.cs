namespace GraphQL.IntrospectionModel;

/// <summary> GraphQL schema. </summary>
public sealed class GraphQLSchema : IHasDescription, IHasDirectives
{
    /// <summary> Gets or sets the schema description. </summary>
    public string? Description { get; set; }

    /// <summary> Gets or sets the type used for query. </summary>
    public GraphQLRequestType? QueryType { get; set; }

    /// <summary> Gets or sets the type used for mutation. </summary>
    public GraphQLRequestType? MutationType { get; set; }

    /// <summary> Gets or sets the type used for subscription. </summary>
    public GraphQLRequestType? SubscriptionType { get; set; }

    /// <summary> Gets or sets the types defined in the schema. </summary>
    public ICollection<GraphQLType> Types { get; set; } = null!;

    /// <summary> Gets or sets the directives defined in the schema. </summary>
    public ICollection<GraphQLDirective>? Directives { get; set; }

    /// <summary> Gets or sets the set of directives applied to the schema. </summary>
    public ICollection<GraphQLAppliedDirective>? AppliedDirectives { get; set; }
}
