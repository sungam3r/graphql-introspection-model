using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.IntrospectionModel.SDL;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.IntrospectionModel.Tests;

public sealed class IntrospectionTests : IClassFixture<GraphQLFixture>
{
    private readonly GraphQLFixture _fixture;

    public IntrospectionTests(GraphQLFixture fixture)
    {
        _fixture = fixture;
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Files", "Introspection", fileName));

    private async Task ShouldMatchAsync(string query, string expected, Action<ExecutionResult>? action = null)
    {
        var executor = _fixture.Provider.GetRequiredService<IDocumentExecuter<TestSchema>>();
        await using var scope = _fixture.Provider.CreateAsyncScope();
        var result = await executor.ExecuteAsync(opt =>
        {
            opt.Query = query;
            opt.RequestServices = scope.ServiceProvider;
        });

        action?.Invoke(result);

        if (result.Errors != null)
            throw new InvalidOperationException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Message + " " + e.InnerException?.Message)));

        var serializer = scope.ServiceProvider.GetRequiredService<IGraphQLTextSerializer>();

        var actual = serializer.Serialize(result);
        var response = JsonSerializer.Deserialize<GraphQLResponse>(actual, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        string sdl = response.ShouldNotBeNull().Data.ShouldNotBeNull().__Schema.ShouldNotBeNull().Print(new ASTConverterOptions { EachDirectiveLocationOnNewLine = true });
        sdl.ShouldBe(expected);
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
        await Should.ThrowAsync<InvalidOperationException>(() => ShouldMatchAsync("query q { qqq }", "-", result =>
        {
            result.Errors.ShouldNotBeNull().Count.ShouldBe(1);
            result.Errors[0].Message.ShouldBe("Cannot query field 'qqq' on type 'Query'.");
        }));
    }
}
