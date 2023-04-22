using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.IntrospectionModel.Tests;

public sealed class GraphQLFixture : IDisposable
{
    public GraphQLFixture()
    {
        var services = new ServiceCollection()
            .AddGraphQL(builder => builder
                .AddSchema<TestSchema>()
                .AddSystemTextJson(opt => opt.WriteIndented = true)
                .AddGraphTypes());

        Provider = services.BuildServiceProvider();
    }

    public ServiceProvider Provider { get; }

    public void Dispose()
    {
        Provider.Dispose();
    }
}
