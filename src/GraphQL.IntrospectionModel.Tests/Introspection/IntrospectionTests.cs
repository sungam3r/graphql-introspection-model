using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.IntrospectionModel.SDL;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.IntrospectionModel.Tests;

public sealed class IntrospectionTests : IClassFixture<GraphQLFixture>
{
    private readonly GraphQLFixture _fixture;
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public IntrospectionTests(GraphQLFixture fixture)
    {
        _fixture = fixture;
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Files", "Introspection", fileName));

    private async Task ShouldMatchAsync(string query, string expected)
    {
        var response = await GetResponseAsync(query);
        string sdl = response.Data.ShouldNotBeNull().__Schema.ShouldNotBeNull().Print(new ASTConverterOptions { EachDirectiveLocationOnNewLine = true });
        sdl.ShouldBe(expected);
    }

    private async Task<GraphQLResponse> GetResponseAsync(string query)
    {
        var executor = _fixture.Provider.GetRequiredService<IDocumentExecuter<TestSchema>>();
        await using var scope = _fixture.Provider.CreateAsyncScope();
        var result = await executor.ExecuteAsync(opt =>
        {
            opt.Query = query;
            opt.RequestServices = scope.ServiceProvider;
        });

        var serializer = scope.ServiceProvider.GetRequiredService<IGraphQLTextSerializer>();

        var actual = serializer.Serialize(result);
        return JsonSerializer.Deserialize<GraphQLResponse>(actual, _options).ShouldNotBeNull();
    }

    [Fact]
    public async Task Classic()
    {
        await ShouldMatchAsync(IntrospectionQuery.Classic, ReadFile("classic.graphql"));
    }

    [Fact]
    public async Task ClassicDraft()
    {
        await ShouldMatchAsync(IntrospectionQuery.ClassicDraft, ReadFile("classic_draft.graphql"));
    }

    [Fact]
    public async Task Modern()
    {
        await ShouldMatchAsync(IntrospectionQuery.Modern, ReadFile("modern.graphql"));
    }

    [Fact]
    public async Task ModernDraft()
    {
        await ShouldMatchAsync(IntrospectionQuery.ModernDraft, ReadFile("modern_draft.graphql"));
    }

    [Fact]
    public async Task Wrong_Query_Should_Return_Errors()
    {
        var response = await GetResponseAsync("query q { qqq }");
        response.Data.ShouldBeNull();
        response.Errors.ShouldNotBeNull().Length.ShouldBe(1);
        response.Errors[0].Message.ShouldBe("Cannot query field 'qqq' on type 'Query'.");
    }
}
