using System;
using System.Linq;
using System.Text;

namespace GraphQL.IntrospectionModel.SDL
{
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

        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly GraphQLSchema _schema;
        private readonly SDLBuilderOptions _options;

        private SDLBuilder(GraphQLSchema schema, SDLBuilderOptions options)
        {
            _schema = schema;
            _options = options;
        }

        private void WriteLine(string text = null, Indent indent = Indent.None)
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

        private void WriteDescription(IHasDescription element, object parent = null)
        {
            if (element.Description != null && IsDescriptionAllowed())
            {
                foreach (string line in element.Description.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    WriteLine("# " + line, indent: GetElementIndent());
            }

            Indent GetElementIndent()
            {
                switch (element)
                {
                    case GraphQLDirective _:
                    case GraphQLType _:
                        return Indent.None;

                    case GraphQLArgument _:
                        return parent is GraphQLDirective ? Indent.Single : Indent.Double;

                    default:
                        return Indent.Single;
                }
            }

            bool IsDescriptionAllowed()
            {
                switch (element)
                {
                    case GraphQLType _:
                        return _options.TypeComments;

                    case GraphQLEnumValue _:
                        return _options.EnumValuesComments;

                    case GraphQLField _:
                    case GraphQLInputField _:
                        return _options.FieldComments;

                    case GraphQLArgument _:
                        return parent is GraphQLDirective ? _options.DirectiveComments : _options.ArgumentComments;

                    case GraphQLDirective _:
                        return _options.DirectiveComments;

                    default:
                        throw new NotSupportedException($"Unknown element '{element.GetType().Name}'");
                }
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

            var typesToBuild = _schema.Types.Where(t => !t.IsIntrospection).OrderBy(t => t.Name, StringComparer.Ordinal).ToArray();
            for (int i = 0; i < typesToBuild.Length; ++i)
            {
                var type = typesToBuild[i];

                WriteDescription(type);

                switch (type.Kind)
                {
                    case GraphQLTypeKind.Enum:
                        WriteEnum(type);
                        break;

                    case GraphQLTypeKind.Input_Object:
                        WriteInput(type);
                        break;

                    case GraphQLTypeKind.Interface:
                        WriteInterface(type);
                        break;

                    case GraphQLTypeKind.Object:
                        WriteObject(type);
                        break;

                    case GraphQLTypeKind.Scalar:
                        WriteScalar(type);
                        break;

                    case GraphQLTypeKind.Union:
                        WriteUnion(type);
                        break;

                    default:
                        throw new NotSupportedException(type.Kind.ToString());
                }

                if (i < typesToBuild.Length - 1)
                    WriteLine();
            }

            return _buffer.ToString();
        }

        private void WriteDirectives()
        {
            if (_schema.Directives == null)
                return;

            foreach (var directive in _schema.Directives.OrderBy(d => d.Name, StringComparer.Ordinal))
            {
                if (IsStandardDirective(directive))
                    continue;

                WriteDescription(directive);

                if (directive.Args == null || directive.Args.Count == 0)
                {
                    WriteLine($"directive @{directive.Name} on");
                }
                else
                {
                    if (directive.Args.All(a => a.Description == null && (a.Directives?.Count ?? 0) == 0))
                    {
                        // if no directive argument has descriptions and directives, then write the entire signature of the directive in one line
                        WriteLine($"directive @{directive.Name}(" + string.Join(", ", directive.Args.Select(arg => $"{arg.Name}: {arg.Type.SDLType}{Default(arg.DefaultValue)}")) + ") on");
                    }
                    else
                    {
                        // otherwise write each argument on a separate line
                        WriteLine($"directive @{directive.Name}(");

                        foreach (var arg in directive.Args)
                        {
                            WriteDescription(arg, directive);
                            WriteLine($"{arg.Name}: {arg.Type.SDLType}{Default(arg.DefaultValue)}{Directives(arg)}", indent: Indent.Single);
                        }

                        WriteLine(") on");
                    }
                }

                foreach (var location in directive.Locations)
                    WriteLine($"| {location.ToString().ToUpperInvariant()}", Indent.Single);

                WriteLine();
            }

            // https://github.com/graphql/graphql-spec/issues/632
            static bool IsStandardDirective(GraphQLDirective directive) => directive.Name == "skip" || directive.Name == "include" || directive.Name == "deprecated";
        }

        private void WriteSchema()
        {
            WriteLine("schema {");

            if (_schema.QueryType != null)
                WriteLine($"query: {_schema.QueryType.Name}", indent: Indent.Single); // generally speaking, query should always be according to the specification 
            if (_schema.MutationType != null)
                WriteLine($"mutation: {_schema.MutationType.Name}", indent: Indent.Single);
            if (_schema.SubscriptionType != null)
                WriteLine($"subscription: {_schema.SubscriptionType.Name}", indent: Indent.Single);

            WriteLine("}");
            WriteLine();
        }

        private void WriteScalar(GraphQLType type)
        {
            WriteLine($"scalar {type.Name}{Directives(type)}");
        }

        private void WriteEnum(GraphQLType type)
        {
            WriteLine($"enum {type.Name}{Directives(type)} {{");

            foreach (var enumValue in type.EnumValues)
            {
                WriteDescription(enumValue);
                WriteLine($"{enumValue.Name}{Deprecate(enumValue)}{Directives(enumValue)}", indent: Indent.Single);
            }

            WriteLine("}");
        }

        private void WriteInput(GraphQLType type)
        {
            WriteLine($"input {type.Name}{Directives(type)} {{");

            foreach (var field in type.InputFields)
            {
                WriteDescription(field);
                WriteLine($"{field.Name}: {field.Type.SDLType}{Default(field.DefaultValue)}{Directives(field)}", indent: Indent.Single);
            }

            WriteLine("}");
        }

        private void WriteInterface(GraphQLType type)
        {
            WriteLine($"interface {type.Name}{Directives(type)} {{");
            WriteObjectOrInterfaceBody(type);
            WriteLine("}");
        }

        private void WriteObject(GraphQLType type)
        {
            WriteLine($"type {type.Name}{Implements(type)}{Directives(type)} {{");
            WriteObjectOrInterfaceBody(type);
            WriteLine("}");
        }

        private void WriteUnion(GraphQLType type)
        {
            WriteLine($"union {type.Name} = {string.Join(" | ", type.PossibleTypes.Select(t => t.Name))}{Directives(type)}");
        }

        private void WriteObjectOrInterfaceBody(GraphQLType type)
        {
            foreach (var field in type.Fields)
            {
                WriteDescription(field);

                if (field.Args == null || field.Args.All(arg => arg.Description == null && (arg.Directives?.Count ?? 0) == 0))
                {
                    // if no field argument has descriptions and directives, then write the entire field signature in one line
                    WriteLine($"{field.Name}{Arguments(field)}: {field.Type.SDLType}{Deprecate(field)}{Directives(field)}", indent: Indent.Single);
                }
                else
                {
                    // otherwise write each argument on a separate line
                    WriteLine($"{field.Name}(", indent: Indent.Single);

                    foreach (var arg in field.Args)
                    {
                        WriteDescription(arg);
                        WriteLine($"{arg.Name}: {arg.Type.SDLType}{Default(arg.DefaultValue)}{Directives(arg)}", indent: Indent.Double);
                    }

                    WriteLine($"): {field.Type.SDLType}{Deprecate(field)}{Directives(field)}", indent: Indent.Single);
                }
            }
        }

        private static string Implements(GraphQLType type)
        {
            return type.Interfaces?.Count > 0
                ? $" implements {string.Join(", ", type.Interfaces.Select(i => i.Name))}"
                : string.Empty;
        }

        private static string Arguments(GraphQLField field)
        {
            return field.Args?.Count > 0
                ? "(" + string.Join(", ", field.Args.Select(arg => $"{arg.Name}: {arg.Type.SDLType}{Default(arg.DefaultValue)}")) + ")"
                : string.Empty;
        }

        private static string Default(object defaultValue)
        {
            // HACK https://github.com/graphql-dotnet/graphql-dotnet/issues/1055
            if (defaultValue?.ToString() == "null")
                return string.Empty;

            if (defaultValue != null)
                return $" = {defaultValue}";

            return string.Empty;
        }

        // https://graphql.github.io/graphql-spec/June2018/#sec--deprecated
        private static string Deprecate(IDeprecatable deprecatable)
        {
            if (deprecatable.IsDeprecated)
            {
                if (deprecatable.DeprecationReason != null)
                    return $" @deprecated(reason: \"{deprecatable.DeprecationReason}\")";
                else
                    return " @deprecated";
            }

            return string.Empty;
        }

        private string Directives(IHasDirectives element)
        {
            return !_options.Directives || (element.Directives?.Count ?? 0) == 0
                ? string.Empty
                : string.Concat(element.Directives.Select(d => $" @{d.Name}{Arguments(d)}"));

            string Arguments(GraphQLDirectiveUsage usage)
            {
                if (usage.Args == null || usage.Args.Count == 0)
                    return string.Empty;

                return $"({string.Join(", ", usage.Args.Select(a => $"{a.Name}: {ToLiteral(usage, a)}"))})";
            }

            string ToLiteral(GraphQLDirectiveUsage usage, GraphQLDirectiveArgument argument)
            {
                var directive = _schema.Directives.First(d => d.Name == usage.Name);
                var arg = directive.Args.First(a => a.Name == argument.Name);
                return GetLiteral(arg.Type, argument.Value);
            }

            static string GetLiteral(GraphQLFieldType type, string value)
            {
                switch (type.Kind)
                {
                    case GraphQLTypeKind.Non_Null:
                        return GetLiteral(type.OfType, value);

                    case GraphQLTypeKind.List:
                        return $"[{GetLiteral(type.OfType, value)}]";

                    case GraphQLTypeKind.Scalar:
                        switch (type.Name)
                        {
                            case "String":
                            case "Uri":
                            case "Id":
                                return "\"" + value + "\"";

                            default:
                                return value;
                        }

                    case GraphQLTypeKind.Enum:
                    default:
                        return value; // TODO: let it be for now
                }
            }
        }
    }
}
