using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel.SDL;

/// <summary>
/// Converts introspection representations into AST representations.
/// </summary>
public partial class ASTConverter
{
    private static readonly List<string> _builtInScalars = new()
    {
        "String",
        "Boolean",
        "Int",
        "Float",
        "ID"
    };

    private readonly ASTConverterOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="ASTConverter"/> with default options.
    /// </summary>
    public ASTConverter()
        : this(new ASTConverterOptions())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ASTConverter"/> with the specified options.
    /// </summary>
    public ASTConverter(ASTConverterOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Converts <see cref="GraphQLSchema"/> into <see cref="GraphQLDocument"/>.
    /// </summary>
    public virtual GraphQLDocument ToDocument(GraphQLSchema schema)
    {
        IEnumerable<GraphQLDirective> SortedDirectives()
        {
            if (schema.Directives == null)
                return Array.Empty<GraphQLDirective>();

            if (_options.DirectiveComparer == null)
                return schema.Directives;

            var copy = schema.Directives.ToList();
            copy.Sort(_options.DirectiveComparer);
            return copy;
        }

        IEnumerable<GraphQLType> SortedTypes()
        {
            if (schema.Types == null)
                return Array.Empty<GraphQLType>();

            var filtered = schema.Types.Where(t => !t.IsIntrospection).ToList();

            if (_options.TypeComparer != null)
                filtered.Sort(_options.TypeComparer);

            return filtered;
        }

        var definitions = new List<ASTNode>();
        definitions.AddIfNotNull(ToSchemaDefinition(schema));

        if (schema.Directives?.Count > 0)
        {
            foreach (var directive in SortedDirectives())
                definitions.AddIfNotNull(ToDirectiveDefinition(directive));
        }

        foreach (var type in SortedTypes())
        {
            definitions.AddIfNotNull(ToTypeDefinition(type));
        }

        return new GraphQLDocument(definitions);
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLTypeDefinition? ToTypeDefinition(GraphQLType type)
    {
        return type.Kind switch
        {
            GraphQLTypeKind.OBJECT => ToObjectTypeDefinition(type),
            GraphQLTypeKind.UNION => ToUnionTypeDefinition(type),
            GraphQLTypeKind.INTERFACE => ToInterfaceTypeDefinition(type),
            GraphQLTypeKind.INPUT_OBJECT => ToInputObjectTypeDefinition(type),
            GraphQLTypeKind.ENUM => ToEnumTypeDefinition(type),
            GraphQLTypeKind.SCALAR => ToScalarTypeDefinition(type),
            _ => throw new NotSupportedException(type.Kind.ToString()),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLObjectTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLObjectTypeDefinition? ToObjectTypeDefinition(GraphQLType type)
    {
        return new GraphQLObjectTypeDefinition(new GraphQLName(type.Name))
        {
            Description = ToDescription(type),
            Directives = ToDirectives(type),
            Fields = ToFieldsDefinition(type.Fields),
            Interfaces = ToImplementsInterfaces(type.Interfaces),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLInterfaceTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLInterfaceTypeDefinition? ToInterfaceTypeDefinition(GraphQLType type)
    {
        return new GraphQLInterfaceTypeDefinition(new GraphQLName(type.Name))
        {
            Description = ToDescription(type),
            Directives = ToDirectives(type),
            Fields = ToFieldsDefinition(type.Fields),
            Interfaces = ToImplementsInterfaces(type.Interfaces),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLField"/> into <see cref="GraphQLFieldDefinition"/>.
    /// </summary>
    public virtual GraphQLFieldDefinition? ToFieldDefinition(GraphQLField field)
    {
        return new GraphQLFieldDefinition(new GraphQLName(field.Name), ToType(field.Type))
        {
            Description = ToDescription(field),
            Directives = ToDirectives(field),
            Arguments = ToArgumentsDefinition(field.Args),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLArgument"/> into <see cref="GraphQLInputValueDefinition"/>.
    /// </summary>
    public virtual GraphQLInputValueDefinition ToInputValueDefinition(GraphQLArgument argument)
    {
        return new GraphQLInputValueDefinition(new GraphQLName(argument.Name), ToType(argument.Type))
        {
            Description = ToDescription(argument),
            Directives = ToDirectives(argument),
            DefaultValue = ToValue(argument.DefaultValue),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLInputField"/> into <see cref="GraphQLInputValueDefinition"/>.
    /// </summary>
    public virtual GraphQLInputValueDefinition ToInputValueDefinition(GraphQLInputField field)
    {
        return new GraphQLInputValueDefinition(new GraphQLName(field.Name), ToType(field.Type))
        {
            Description = ToDescription(field),
            Directives = ToDirectives(field),
            DefaultValue = ToValue(field.DefaultValue),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLFieldType"/> into <see cref="GraphQLParser.AST.GraphQLType"/>.
    /// </summary>
    public virtual GraphQLParser.AST.GraphQLType ToType(GraphQLFieldType type)
    {
        return type.Kind switch
        {
            GraphQLTypeKind.LIST => new GraphQLListType(ToType(type.OfType!)),
            GraphQLTypeKind.NON_NULL => new GraphQLNonNullType(ToType(type.OfType!)),
            _ => new GraphQLNamedType(new GraphQLName(type.Name)),
        };
    }

    /// <summary>
    /// Converts GraphQL-formatted string representation of value literal into <see cref="GraphQLValue"/>.
    /// </summary>
    public virtual GraphQLValue? ToValue(string? value)
    {
        return value == null ? null : Parser.Parse<GraphQLValue>(value);
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLInputObjectTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLInputObjectTypeDefinition? ToInputObjectTypeDefinition(GraphQLType type)
    {
        return new GraphQLInputObjectTypeDefinition(new GraphQLName(type.Name))
        {
            Description = ToDescription(type),
            Directives = ToDirectives(type),
            Fields = ToInputFieldsDefinition(type.InputFields),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLUnionTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLUnionTypeDefinition? ToUnionTypeDefinition(GraphQLType type)
    {
        return new GraphQLUnionTypeDefinition(new GraphQLName(type.Name))
        {
            Description = ToDescription(type),
            Directives = ToDirectives(type),
            Types = ToUnionMemberTypes(type.PossibleTypes),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLEnumTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLEnumTypeDefinition? ToEnumTypeDefinition(GraphQLType type)
    {
        return new GraphQLEnumTypeDefinition(new GraphQLName(type.Name))
        {
            Description = ToDescription(type),
            Directives = ToDirectives(type),
            Values = ToEnumValuesDefinition(type.EnumValues),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLEnumValue"/> into <see cref="GraphQLEnumValueDefinition"/>.
    /// </summary>
    public virtual GraphQLEnumValueDefinition? ToEnumValueDefinition(GraphQLEnumValue value)
    {
        return new GraphQLEnumValueDefinition(new GraphQLName(value.Name), new GraphQLParser.AST.GraphQLEnumValue(new GraphQLName(value.Name)))
        {
            Description = ToDescription(value),
            Directives = ToDirectives(value),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLType"/> into <see cref="GraphQLScalarTypeDefinition"/>.
    /// </summary>
    public virtual GraphQLScalarTypeDefinition? ToScalarTypeDefinition(GraphQLType type)
    {
        return _options.OmitBuiltInScalars && _builtInScalars.Contains(type.Name)
            ? null
            : new GraphQLScalarTypeDefinition(new GraphQLName(type.Name))
            {
                Description = ToDescription(type),
                Directives = ToDirectives(type),
            };
    }

    /// <summary>
    /// Converts <see cref="GraphQLDirective"/> into <see cref="GraphQLDirectiveDefinition"/>.
    /// </summary>
    public virtual GraphQLDirectiveDefinition? ToDirectiveDefinition(GraphQLDirective directive)
    {
        // https://github.com/graphql/graphql-spec/issues/632
        static bool IsStandardDirective(GraphQLDirective directive) => directive.Name == "skip" || directive.Name == "include" || directive.Name == "deprecated";

        return IsStandardDirective(directive)
            ? null
            : new GraphQLDirectiveDefinition(new GraphQLName(directive.Name), ToDirectiveLocations(directive.Locations))
            {
                Description = ToDescription(directive),
                Arguments = ToArgumentsDefinition(directive.Args),
                Repeatable = directive.IsRepeatable,
            };
    }

    /// <summary>
    /// Converts any implementation of <see cref="IHasDescription"/> into <see cref="GraphQLDescription"/>.
    /// </summary>
    public virtual GraphQLDescription? ToDescription(IHasDescription element)
    {
        return element.Description == null || !_options.PrintDescriptions
            ? null
            : new GraphQLDescription(element.Description);
    }

    /// <summary>
    /// Converts <see cref="GraphQLSchema"/> into <see cref="GraphQLSchemaDefinition"/>.
    /// </summary>
    public virtual GraphQLSchemaDefinition? ToSchemaDefinition(GraphQLSchema schema)
    {
        return (schema.AppliedDirectives == null || !_options.PrintAppliedDirectives) && schema.QueryType == null && schema.MutationType == null && schema.SubscriptionType == null
            ? null
            : new GraphQLSchemaDefinition(
                new List<GraphQLRootOperationTypeDefinition>()
                    .AddIfNotNull(schema.QueryType == null ? null : new GraphQLRootOperationTypeDefinition { Operation = OperationType.Query, Type = new GraphQLNamedType(new GraphQLName(schema.QueryType.Name)) })
                    .AddIfNotNull(schema.MutationType == null ? null : new GraphQLRootOperationTypeDefinition { Operation = OperationType.Mutation, Type = new GraphQLNamedType(new GraphQLName(schema.MutationType.Name)) })
                    .AddIfNotNull(schema.SubscriptionType == null ? null : new GraphQLRootOperationTypeDefinition { Operation = OperationType.Subscription, Type = new GraphQLNamedType(new GraphQLName(schema.SubscriptionType.Name)) }))
            {
                Description = ToDescription(schema),
                Directives = ToDirectives(schema),
            };
    }

    /// <summary>
    /// Converts <see cref="GraphQLAppliedDirective"/> into <see cref="GraphQLParser.AST.GraphQLDirective"/>.
    /// </summary>
    public virtual GraphQLParser.AST.GraphQLDirective? ToDirective(GraphQLAppliedDirective directive)
    {
        return new GraphQLParser.AST.GraphQLDirective(new GraphQLName(directive.Name))
        {
            Arguments = ToArguments(directive.Args),
        };
    }

    /// <summary>
    /// Converts <see cref="GraphQLDirectiveArgument"/> into <see cref="GraphQLParser.AST.GraphQLArgument"/>.
    /// </summary>
    public virtual GraphQLParser.AST.GraphQLArgument? ToArgument(GraphQLDirectiveArgument argument)
    {
        return new GraphQLParser.AST.GraphQLArgument(new GraphQLName(argument.Name), ToValue(argument.Value)!); //TODO: ! operator
    }
}
