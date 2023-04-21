using System.Diagnostics;

namespace GraphQL.IntrospectionModel;

/// <summary> A field of some input type. </summary>
[DebuggerDisplay("Input field {Name,nq}")]
public sealed class GraphQLInputField : GraphQLInputValue
{
}
