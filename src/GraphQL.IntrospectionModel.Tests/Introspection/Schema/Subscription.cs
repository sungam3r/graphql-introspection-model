using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class Subscription : ObjectGraphType
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Test")]
    public Subscription()
    {
        Field<ListGraphType<IntGraphType>>("values").Resolve(_ => new[] { 1, 2 });
    }
}
