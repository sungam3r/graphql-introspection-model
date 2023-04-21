namespace GraphQL.IntrospectionModel;

/// <summary>
/// This type represents field and directive arguments as well as the input fields of an input object.
/// </summary>
public abstract class GraphQLInputValue : GraphQLNamedObject, IDeprecatable
{
    /// <summary> Gets or sets the type of this input value. </summary>
    public GraphQLFieldType Type { get; set; } = null!;

    /// <summary> Gets or sets a GraphQL-formatted string representing the default value for this input value. </summary>
    public string? DefaultValue { get; set; }

    /// <summary> Gets or sets a value indicating whether this input value is deprecated. </summary>
    public bool IsDeprecated { get; set; }

    /// <summary> Gets or sets the reason for why input value is deprecated. </summary>
    public string? DeprecationReason { get; set; }
}
