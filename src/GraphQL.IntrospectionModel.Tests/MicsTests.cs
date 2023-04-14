namespace GraphQL.IntrospectionModel.Tests;

public class MicsTests
{
    [Fact]
    public void GraphQLFieldType_IsIntrospection()
    {
        new GraphQLFieldType { Name = "__abc" }.IsIntrospection.ShouldBeTrue();
        new GraphQLFieldType { Name = "abc" }.IsIntrospection.ShouldBeFalse();
        new GraphQLFieldType { }.IsIntrospection.ShouldBeFalse();
    }
}
