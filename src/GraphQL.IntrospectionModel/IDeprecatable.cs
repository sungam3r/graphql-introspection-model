namespace GraphQL.IntrospectionModel;

/// <summary> The interface for those parts of the schema that can be declared deprecated: https://graphql.github.io/graphql-spec/June2018/#sec--deprecated. </summary>
public interface IDeprecatable
{
    /// <summary> Gets or sets a value indicating whether the item is deprecated. </summary>
    bool IsDeprecated { get; set; }

    /// <summary> Gets or sets the reason for why item is deprecated. </summary>
    string? DeprecationReason { get; set; }
}
