using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class AuthorDirective : Directive
{
    public AuthorDirective()
        : base("author", DirectiveLocation.FieldDefinition, DirectiveLocation.Object)
    {
        Description = "Author's name";
        Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>>
        {
            Name = "name"
        });
    }

    public override bool? Introspectable => true;
}
