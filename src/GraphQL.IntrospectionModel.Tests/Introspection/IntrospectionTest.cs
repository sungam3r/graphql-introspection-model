using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.IntrospectionModel.SDL;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

public class IntegrationTest : IDisposable
{
    private readonly ServiceProvider _provider;

    public IntegrationTest()
    {
        var services = new ServiceCollection()
            .AddGraphQL(builder => builder
                .AddSchema<TestSchema>()
                .AddSystemTextJson(opt => opt.WriteIndented = true)
                .AddGraphTypes());

        _provider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _provider.Dispose();
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Introspection", fileName));

    private async Task ShouldMatchAsync(string query, string expected)
    {
        var executor = _provider.GetRequiredService<IDocumentExecuter<TestSchema>>();
        using var scope = _provider.CreateScope();
        var result = await executor.ExecuteAsync(opt =>
        {
            opt.Query = query;
            opt.RequestServices = scope.ServiceProvider;
        });

        if (result.Errors != null)
            throw new InvalidOperationException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Message + " " + e.InnerException?.Message)));

        var serializer = _provider.GetRequiredService<IGraphQLTextSerializer>();

        var actual = serializer.Serialize(result);
        var schemaElement = JsonDocument.Parse(actual).RootElement.GetProperty("data").GetProperty("__schema");
        var model = JsonSerializer.Deserialize<GraphQLSchema>(schemaElement, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
        string sdl = SDLBuilder.Build(model!);
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
}
