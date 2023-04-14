using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

internal class MyDirective : Directive
{
    public MyDirective()
        : base("my", DirectiveLocation.FieldDefinition)
    {
        Description = "test";
        Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>>
        {
            Name = "arg"
        });
    }

    public override bool? Introspectable => true;
}
