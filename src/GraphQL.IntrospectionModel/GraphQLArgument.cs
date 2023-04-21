using System.Diagnostics;

namespace GraphQL.IntrospectionModel;

/// <summary> An argument to a field or directive. </summary>
[DebuggerDisplay("Argument '{Name,nq}'")]
public sealed class GraphQLArgument : GraphQLInputValue
{
}
