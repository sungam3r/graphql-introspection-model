using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

internal class Query : ObjectGraphType
{
    public Query()
    {
        Field<StringGraphType>("hello").Resolve(_ => "Hello, World!").Directive("my", "arg", "value");
    }
}
