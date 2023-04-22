using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class Subscription : ObjectGraphType
{
    public Subscription()
    {
        Field<ListGraphType<IntGraphType>>("values").Resolve(_ => new[] { 1, 2 });
    }
}
