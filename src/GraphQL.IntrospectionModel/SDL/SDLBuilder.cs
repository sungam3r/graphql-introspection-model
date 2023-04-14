using System.Text;

namespace GraphQL.IntrospectionModel.SDL;

/// <summary> SDL Schema Builder. Generates SDL according to the introspection request. </summary>
public class SDLBuilder
{
    /// <summary> Indentation level. </summary>
    private enum Indent
    {
        /// <summary> A line without indentation. </summary>
        None = 0,

        /// <summary> Single indented line. </summary>
        Single = 1,

        /// <summary> Double indented line. </summary>
        Double = 2,
    }

    private static readonly List<string> _builtInScalars = new()
    {
        "String",
        "Boolean",
        "Int",
        "Float",
        "ID"
    };
    private static readonly char[] _escapedCharacters = new char[] { '"', '\\', '\b', '\f', '\n', '\r', '\t' };
    private readonly StringBuilder _buffer = new();
    private readonly GraphQLSchema _schema;
    private readonly SDLBuilderOptions _options;
    private bool _appendNewLine;

    private SDLBuilder(GraphQLSchema schema, SDLBuilderOptions options)
    {
        _schema = schema;
        _options = options;
    }

    private void HandleNewLine()
    {
        if (_appendNewLine)
            WriteLine();
        else
            _appendNewLine = true;
    }

    // https://graphql.github.io/graphql-spec/October2021/#sec-String-Value
    private static string EscapeString(string source)
    {
        if (source.IndexOfAny(_escapedCharacters) == -1)
            return source;

        var buffer = new StringBuilder(source.Length + 10);
        foreach (char character in source)
        {
            switch (character)
            {
                case '"':
                    buffer.Append("\\\"");
                    break;
                case '\\':
                    buffer.Append("\\\\");
                    break;
                case '\b':
                    buffer.Append("\\b");
                    break;
                case '\f':
                    buffer.Append("\\f");
                    break;
                case '\n':
                    buffer.Append("\\n");
                    break;
                case '\r':
                    buffer.Append("\\r");
                    break;
                case '\t':
                    buffer.Append("\\t");
                    break;
                default:
                    buffer.Append(character);
                    break;
            }
        }

        return buffer.ToString();
    }

    private void WriteLine(string? text = null, Indent indent = Indent.None)
    {
        if (text != null)
        {
            for (int i = 0; i < _options.IndentSize * (int)indent; ++i)
                _buffer.Append(' ');

            _buffer.AppendLine(text);
        }
        else
        {
            _buffer.AppendLine();
        }
    }

    private void WriteDescription(IHasDescription element, object? parent = null)
    {
        if (element.Description != null && IsDescriptionAllowed())
        {
            foreach (string line in element.Description.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                WriteLine("# " + EscapeString(line), indent: GetElementIndent());
        }

        Indent GetElementIndent()
        {
            return element switch
            {
                GraphQLDirective _ or GraphQLType _ => Indent.None,
                GraphQLArgument _ => parent is GraphQLDirective ? Indent.Single : Indent.Double,
                _ => Indent.Single,
            };
        }

        bool IsDescriptionAllowed()
        {
            return element switch
            {
                GraphQLType _ => _options.TypeComments,
                GraphQLEnumValue _ => _options.EnumValuesComments,
                GraphQLField _ or GraphQLInputField _ => _options.FieldComments,
                GraphQLArgument _ => parent is GraphQLDirective ? _options.DirectiveComments : _options.ArgumentComments,
                GraphQLDirective _ => _options.DirectiveComments,
                _ => throw new NotSupportedException($"Unknown element '{element.GetType().Name}'"),
            };
        }
    }

    /// <summary> Build the SDL schema using the GraphQL schema from introspection request using the default output settings. </summary>
    /// <param name="schema"> GraphQL schema. </param>
    /// <returns> SDL schema. </returns>
    public static string Build(GraphQLSchema schema) => Build(schema, new SDLBuilderOptions());

    /// <summary> Build the SDL schema using the GraphQL schema from introspection request using the specified output options. </summary>
    /// <param name="schema"> GraphQL schema. </param>
    /// <param name="options"> Output options. </param>
    /// <returns> SDL schema. </returns>
    public static string Build(GraphQLSchema schema, SDLBuilderOptions options) => new SDLBuilder(schema, options ?? throw new ArgumentNullException(nameof(options))).Build();

    private string Build()
    {
        WriteDirectives();
        WriteSchema();
        WriteTypes();

        return _buffer.ToString();
    }

    private void WriteDirectives()
    {
        ICollection<GraphQLDirective> SortedDirectives()
        {
            if (_options.DirectiveComparer == null)
                return _schema.Directives!;

            var copy = _schema.Directives.ToList();
            copy.Sort(_options.DirectiveComparer);
            return copy;
        }

        if (_schema.Directives == null || _schema.Directives.Count == 0)
            return;

        foreach (var directive in SortedDirectives())
        {
            if (IsStandardDirective(directive))
                continue;

            HandleNewLine();

            WriteDescription(directive);

            if (directive.Args == null || directive.Args.Count == 0)
            {
                if (directive.IsRepeatable)
                    WriteLine($"directive @{directive.Name} repeatable on");
                else
                    WriteLine($"directive @{directive.Name} on");
            }
            else
            {
                if (directive.Args.All(a => a.Description == null && (a.AppliedDirectives?.Count ?? 0) == 0))
                {
                    // if no directive argument has descriptions and directives, then write the entire signature of the directive in one line
                    WriteLine($"directive @{directive.Name}(" + string.Join(", ", directive.Args.Select(arg => $"{arg.Name}: {arg.Type.SDLType}{PrintDefault(arg.DefaultValue)}")) + ") " + (directive.IsRepeatable ? "repeatable on" : "on"));
                }
                else
                {
                    // otherwise write each argument on a separate line
                    WriteLine($"directive @{directive.Name}(");

                    foreach (var arg in directive.Args)
                    {
                        WriteDescription(arg, directive);
                        WriteLine($"{arg.Name}: {arg.Type.SDLType}{PrintDefault(arg.DefaultValue)}{Directives(arg)}", indent: Indent.Single);
                    }

                    if (directive.IsRepeatable)
                        WriteLine(") repeatable on");
                    else
                        WriteLine(") on");
                }
            }

            if (directive.Locations?.Count > 0)
            {
                foreach (var location in directive.Locations)
                    WriteLine($"| {location.ToString().ToUpperInvariant()}", Indent.Single);
            }
        }

        // https://github.com/graphql/graphql-spec/issues/632
        static bool IsStandardDirective(GraphQLDirective directive) => directive.Name == "skip" || directive.Name == "include" || directive.Name == "deprecated";
    }

    private void WriteSchema()
    {
        // skip printing empty schema { }
        if ((_schema.AppliedDirectives == null || !_options.Directives) && _schema.QueryType == null && _schema.MutationType == null && _schema.SubscriptionType == null)
            return;

        HandleNewLine();

        WriteLine($"schema{Directives(_schema)} {{");

        if (_schema.QueryType != null)
            WriteLine($"query: {_schema.QueryType.Name}", indent: Indent.Single); // generally speaking, query should always be according to the specification
        if (_schema.MutationType != null)
            WriteLine($"mutation: {_schema.MutationType.Name}", indent: Indent.Single);
        if (_schema.SubscriptionType != null)
            WriteLine($"subscription: {_schema.SubscriptionType.Name}", indent: Indent.Single);

        WriteLine("}");
    }

    private void WriteTypes()
    {
        List<GraphQLType> SortedTypes()
        {
            var filtered = _schema.Types.Where(t => !t.IsIntrospection).ToList();

            if (_options.TypeComparer != null)
                filtered.Sort(_options.TypeComparer);

            return filtered;
        }

        if (_schema.Types == null || _schema.Types.Count == 0)
            return;

        var typesToBuild = SortedTypes();
        for (int i = 0; i < typesToBuild.Count; ++i)
        {
            var type = typesToBuild[i];

            switch (type.Kind)
            {
                case GraphQLTypeKind.ENUM:
                    WriteEnum(type);
                    break;

                case GraphQLTypeKind.INPUT_OBJECT:
                    WriteInput(type);
                    break;

                case GraphQLTypeKind.INTERFACE:
                    WriteInterface(type);
                    break;

                case GraphQLTypeKind.OBJECT:
                    WriteObject(type);
                    break;

                case GraphQLTypeKind.SCALAR:
                    WriteScalar(type);
                    break;

                case GraphQLTypeKind.UNION:
                    WriteUnion(type);
                    break;

                default:
                    throw new NotSupportedException(type.Kind.ToString());
            }
        }
    }

    private void WriteScalar(GraphQLType type)
    {
        if (!_options.OmitBuiltInScalars || !_builtInScalars.Contains(type.Name))
        {
            HandleNewLine();
            WriteDescription(type);
            WriteLine($"scalar {type.Name}{Directives(type)}");
        }
    }

    private void WriteEnum(GraphQLType type)
    {
        HandleNewLine();
        WriteDescription(type);
        WriteLine($"enum {type.Name}{Directives(type)} {{");

        foreach (var enumValue in type.EnumValues)
        {
            WriteDescription(enumValue);
            WriteLine($"{enumValue.Name}{Directives(enumValue)}", indent: Indent.Single);
        }

        WriteLine("}");
    }

    private void WriteInput(GraphQLType type)
    {
        HandleNewLine();
        WriteDescription(type);
        WriteLine($"input {type.Name}{Directives(type)} {{");

        if (type.InputFields != null)
        {
            foreach (var field in type.InputFields)
            {
                WriteDescription(field);
                WriteLine($"{field.Name}: {field.Type.SDLType}{PrintDefault(field.DefaultValue)}{Directives(field)}", indent: Indent.Single);
            }
        }

        WriteLine("}");
    }

    private void WriteInterface(GraphQLType type)
    {
        HandleNewLine();
        WriteDescription(type);
        WriteLine($"interface {type.Name}{Directives(type)} {{");
        WriteObjectOrInterfaceBody(type);
        WriteLine("}");
    }

    private void WriteObject(GraphQLType type)
    {
        HandleNewLine();
        WriteDescription(type);
        WriteLine($"type {type.Name}{Implements(type)}{Directives(type)} {{");
        WriteObjectOrInterfaceBody(type);
        WriteLine("}");
    }

    private void WriteUnion(GraphQLType type)
    {
        HandleNewLine();
        WriteDescription(type);
        WriteLine($"union {type.Name} = {string.Join(" | ", type.PossibleTypes.Select(t => t.Name))}{Directives(type)}");
    }

    private void WriteObjectOrInterfaceBody(GraphQLType type)
    {
        if (type.Fields == null)
            return;

        foreach (var field in type.Fields)
        {
            WriteDescription(field);

            if (field.Args == null || field.Args.All(arg => arg.Description == null && (arg.AppliedDirectives?.Count ?? 0) == 0))
            {
                // if no field argument has descriptions and directives, then write the entire field signature in one line
                WriteLine($"{field.Name}{Arguments(field)}: {field.Type.SDLType}{Directives(field)}", indent: Indent.Single);
            }
            else
            {
                // otherwise write each argument on a separate line
                WriteLine($"{field.Name}(", indent: Indent.Single);

                foreach (var arg in field.Args)
                {
                    WriteDescription(arg);
                    WriteLine($"{arg.Name}: {arg.Type.SDLType}{PrintDefault(arg.DefaultValue)}{Directives(arg)}", indent: Indent.Double);
                }

                WriteLine($"): {field.Type.SDLType}{Directives(field)}", indent: Indent.Single);
            }
        }
    }

    private static string Implements(GraphQLType type)
    {
        return type.Interfaces?.Count > 0
            ? $" implements {string.Join(" & ", type.Interfaces.Select(i => i.Name))}"
            : string.Empty;
    }

    private static string Arguments(GraphQLField field)
    {
        return field.Args?.Count > 0
            ? "(" + string.Join(", ", field.Args.Select(arg => $"{arg.Name}: {arg.Type.SDLType}{PrintDefault(arg.DefaultValue)}")) + ")"
            : string.Empty;
    }

    private static string PrintDefault(string? defaultValue)
        => defaultValue == null ? string.Empty : $" = {defaultValue}";

    private string Directives(IHasDirectives element)
    {
        if (!_options.Directives)
            return string.Empty;

        // In case of applied directives (if the server supports them) @deprecated should be always among AppliedDirectives
        // because AppliedDirectives is non-null field.
        if (element.AppliedDirectives != null)
        {
            return element.AppliedDirectives.Count == 0
                ? string.Empty
                : string.Concat(element.AppliedDirectives.Select(d => $" @{d.Name}{Arguments(d)}"));
        }
        // Else fall back to print only @deprecated
        else if (element is IDeprecatable deprecatable)
        {
            // https://graphql.github.io/graphql-spec/October2021/#sec--deprecated
            if (deprecatable.IsDeprecated)
            {
                return deprecatable.DeprecationReason == null
                    ? " @deprecated"
                    : $" @deprecated(reason: \"{EscapeString(deprecatable.DeprecationReason)}\")";
            }

            return string.Empty;
        }
        else
        {
            return string.Empty;
        }

        static string Arguments(GraphQLAppliedDirective applied)
        {
            return applied.Args == null || applied.Args.Count == 0
                ? string.Empty
                : $"({string.Join(", ", applied.Args.Select(a => $"{a.Name}: {a.Value ?? "null"}"))})";
        }
    }
}
