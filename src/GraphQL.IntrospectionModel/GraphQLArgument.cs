using System.Diagnostics;

namespace GraphQL.IntrospectionModel;

/// <summary> An argument to a field or directive. </summary>
[DebuggerDisplay("Argument '{Name,nq}'")]
public sealed class GraphQLArgument : GraphQLNamedObject, IDeprecatable
{
    /// <summary> Gets or sets the type of this argument. </summary>
    public GraphQLFieldType Type { get; set; } = null!;

    /// <summary> Gets or sets a GraphQL-formatted string representing the default value for this argument. </summary>
    public string? DefaultValue { get; set; }

    /// <summary> Gets or sets a value indicating whether this argument is deprecated. </summary>
    public bool IsDeprecated { get; set; }

    /// <summary> Gets or sets the reason for why argument is deprecated. </summary>
    public string? DeprecationReason { get; set; }
}
