namespace GraphQL.IntrospectionModel;

/// <summary>
/// The root object obtained by introspection request.
/// </summary>
public class GraphQLResponse
{
    /// <summary>
    /// Data returned by GraphQL server.
    /// </summary>
    public GraphQLData? Data { get; set; }

    /// <summary>
    /// Errors returned by GraphQL server.
    /// </summary>
    public GraphQLError[]? Errors { get; set; }
}

/// <summary>
/// Container for normal response.
/// </summary>
public class GraphQLData
{
    /// <summary>
    /// GraphQL schema.
    /// </summary>
    public GraphQLSchema? __Schema { get; set; }
}

/// <summary>
/// Error returned by GraphQL server.
/// </summary>
public sealed class GraphQLError
{
    /// <summary>
    /// Error message.
    /// </summary>
    public string? Message { get; set; }
}
