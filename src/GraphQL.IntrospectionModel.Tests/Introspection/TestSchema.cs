using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

internal class TestSchema : Schema
{
    public TestSchema(IServiceProvider provider) : base(provider)
    {
        this.EnableExperimentalIntrospectionFeatures(ExperimentalIntrospectionFeaturesMode.IntrospectionAndExecution);

        Query = new Query();
        Mutation = new Mutation();
        Subscription = new Subscription();

        Directives.Register(new MyDirective());
    }
}
