namespace GraphQL.IntrospectionModel.SDL;

/// <summary> Output options for creating SDL schemas. </summary>
public sealed class SDLBuilderOptions
{
    /// <summary> Gets or sets a value indicating whether to display comments for schema in the generated SDL. </summary>
    public bool SchemaComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to display comments for types in the generated SDL. </summary>
    public bool TypeComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to display comments for fields in the generated SDL. </summary>
    public bool FieldComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to display comments for enumeration values in the generated SDL. </summary>
    public bool EnumValuesComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to display comments for field arguments in the generated SDL. </summary>
    public bool ArgumentComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to display comments for directives and their arguments in the generated SDL. </summary>
    public bool DirectiveComments { get; set; } = true;

    /// <summary> Gets or sets a value indicating whether to output applied directives in the generated SDL. </summary>
    public bool AppliedDirectives { get; set; } = true;

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

    /// <summary> Gets or sets the size of the indentation in spaces in the generated SDL. </summary>
    public int IndentSize { get; set; } = 2;

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
