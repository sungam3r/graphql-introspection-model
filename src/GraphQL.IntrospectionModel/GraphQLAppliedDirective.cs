using System.Diagnostics;

namespace GraphQL.IntrospectionModel;

/// <summary>
/// A directive applied to some element of a schema. This is not a directive declaration - <see cref="GraphQLDirective"/>,
/// but rather its application inside the schema. Only TypeSystemDirectiveLocation directives can be used this way.
/// </summary>
/// <remarks>
/// This is an experimental type, the official specification does not contain such a concept (yet).
/// </remarks>
[DebuggerDisplay("DirectiveUsage '{Name,nq}'")]
public sealed class GraphQLAppliedDirective
{
    /// <summary> Gets or sets the name of the applied directive. </summary>
    public string Name { get; set; } = null!;

    /// <summary> Gets or sets the argument list (argument names and values) of the applied directive. </summary>
    public ICollection<GraphQLDirectiveArgument>? Args { get; set; }
}
