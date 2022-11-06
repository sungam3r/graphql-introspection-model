using System.Diagnostics;

namespace GraphQL.IntrospectionModel;

/// <summary> The value of the enumeration. </summary>
[DebuggerDisplay("{Name,nq}")]
public sealed class GraphQLEnumValue : GraphQLNamedObject, IDeprecatable
{
    /// <summary> Gets or sets a value indicating whether the value of the enumeration is deprecated. </summary>
    public bool IsDeprecated { get; set; }

    /// <summary> Gets or sets the reason for why value is deprecated. </summary>
    public string? DeprecationReason { get; set; }
}
