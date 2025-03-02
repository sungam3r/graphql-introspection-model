using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class Query : ObjectGraphType
{
    public Query()
    {
        Field<StringGraphType>("hello").Resolve(_ => "Hello, World!").ApplyDirective("author", "name", "Alice");
        Field<NonNullGraphType<StringGraphType>>("word").Resolve(_ => "abcdef").ApplyDirective("revert").ApplyDirective("revert");
        Field<CatOrDogGraphType>("catOrDog");
    }
}

public class CatOrDogGraphType : UnionGraphType
{
    public CatOrDogGraphType()
    {
        Type<DogGraphType>();
        Type<CatGraphType>();

        ResolveType = value => null;
    }
}

public class DogGraphType : ObjectGraphType
{
    public DogGraphType()
    {
        Field<StringGraphType>("nickname");
        Field<BooleanGraphType>("barks");
        Field<IntGraphType>("barkVolume");
        IsTypeOf = _ => true;
    }
}

public class CatGraphType : ObjectGraphType
{
    public CatGraphType()
    {
        Field<StringGraphType>("nickname");
        Field<BooleanGraphType>("meows");
        Field<IntGraphType>("meowVolume");
        IsTypeOf = _ => true;
    }
}
