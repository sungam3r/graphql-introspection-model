using PublicApiGenerator;
using Shouldly;
using Xunit;

namespace GraphQL.IntrospectionModel.Tests.Approval;

/// <summary> Tests for checking changes to the public API. </summary>
public class ApiApprovalTests
{
    /// <summary> Check for changes to the public APIs. </summary>
    /// <param name="type"> The type used as a marker for the assembly whose public API change you want to check. </param>
    [Theory]
    [InlineData(typeof(GraphQLField))]
    public void PublicApi(Type type)
    {
        string publicApi = type.Assembly.GeneratePublicApi(new()
        {
            IncludeAssemblyAttributes = false,
            AllowNamespacePrefixes = new[] { "Microsoft.Extensions.DependencyInjection" },
            ExcludeAttributes = new[] { "System.Diagnostics.DebuggerDisplayAttribute" },
        });

        publicApi.ShouldMatchApproved(options => options!.WithFilenameGenerator((testMethodInfo, discriminator, fileType, fileExtension) => $"{type.Assembly.GetName().Name!}.{fileType}.{fileExtension}"));
    }
}
