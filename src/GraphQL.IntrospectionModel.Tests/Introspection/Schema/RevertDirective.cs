using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel.Tests;

internal sealed class RevertDirective : Directive
{
    public RevertDirective()
        : base("revert", DirectiveLocation.FieldDefinition)
    {
        Description = "Reverts field value";
        Repeatable = true;
    }

    public override bool? Introspectable => true;

}
