namespace GraphQL.IntrospectionModel.SDL
{
    /// <summary> Output options for creating SDL schemas. </summary>
    public sealed class SDLBuilderOptions
    {
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

        /// <summary> Gets or sets a value indicating whether to output directives in the generated SDL. </summary>
        public bool Directives { get; set; } = true;

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
    }
}
