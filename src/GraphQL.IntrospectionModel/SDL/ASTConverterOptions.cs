using GraphQLParser.Visitors;

namespace GraphQL.IntrospectionModel.SDL;

/// <summary>
/// Options for <see cref="ASTConverter"/>.
/// </summary>
public sealed class ASTConverterOptions : SDLPrinterOptions
{
    /// <summary>
    /// Print descriptions into the output.
    /// </summary>
    public bool PrintDescriptions { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to output applied directives in the generated SDL.
    /// @deprecated directive (if any) is always printed regardless of the value of this property.
    /// </summary>
    public bool PrintAppliedDirectives { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to omit built-in scalars from the generated SDL.
    /// By default <see langword="true"/>.
    ///<br/><br/>
    /// From the spec:
    /// <br/>
    /// When representing a GraphQL schema using the type system definition language, the built‐in
    /// scalar types should be omitted for brevity.
    /// </summary>
    public bool OmitBuiltInScalars { get; set; } = true;

    /// <summary>
    /// Comparer to sort directives in the generated SDL.
    /// If not set directives are not sorted at all.
    /// By default directives are sorted in alphabet order.
    /// </summary>
    public IComparer<GraphQLDirective>? DirectiveComparer { get; set; } = Comparer<GraphQLDirective>.Create((a, b) => string.Compare(a.Name, b.Name, ignoreCase: true));

    /// <summary>
    /// Comparer to sort types in the generated SDL.
    /// If not set types are not sorted at all.
    /// By default types are sorted in alphabet order.
    /// </summary>
    public IComparer<GraphQLType>? TypeComparer { get; set; } = Comparer<GraphQLType>.Create((a, b) => string.Compare(a.Name, b.Name, ignoreCase: true));
}
