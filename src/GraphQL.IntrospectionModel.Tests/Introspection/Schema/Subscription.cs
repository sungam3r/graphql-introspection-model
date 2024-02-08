using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class Subscription : ObjectGraphType
{
    public Subscription()
    {
        Field<ListGraphType<IntGraphType>>("values").ResolveStream(_ => new StubObservable());
    }

    internal sealed class StubObservable : IObservable<object?>
    {
        public IDisposable Subscribe(IObserver<object?> observer) => throw new NotSupportedException();
    }
}
