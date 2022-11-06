namespace GraphQL.IntrospectionModel;

/// <summary> The base class for the named GraphQL parts of the schema. </summary>
public abstract class GraphQLNamedObject : IHasDescription, IHasDirectives
{
    /// <summary> Gets or sets the name of the element. </summary>
    public string Name { get; set; } = null!;

    /// <summary> Gets or sets the description of the element. </summary>
    public string? Description { get; set; }

    /// <summary> Gets or sets the set of directives applied to the element. </summary>
    public ICollection<GraphQLAppliedDirective>? AppliedDirectives { get; set; }
}
