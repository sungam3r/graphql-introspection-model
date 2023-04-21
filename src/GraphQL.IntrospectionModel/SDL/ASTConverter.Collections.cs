using GraphQLParser.AST;

namespace GraphQL.IntrospectionModel.SDL;

public partial class ASTConverter
{
    /// <summary>
    /// Converts collection of <see cref="GraphQLFieldType"/> into <see cref="GraphQLEnumValuesDefinition"/>.
    /// </summary>
    public virtual GraphQLEnumValuesDefinition? ToEnumValuesDefinition(ICollection<GraphQLEnumValue>? values)
    {
        if (values?.Count > 0)
        {
            var definitions = new GraphQLEnumValuesDefinition(new());
            foreach (var value in values)
                definitions.Items.AddIfNotNull(ToEnumValueDefinition(value));

            return definitions;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLFieldType"/> into <see cref="GraphQLUnionMemberTypes"/>.
    /// </summary>
    public virtual GraphQLUnionMemberTypes? ToUnionMemberTypes(ICollection<GraphQLFieldType>? types)
    {
        if (types?.Count > 0)
        {
            var memberTypes = new GraphQLUnionMemberTypes(new());
            foreach (var type in types)
                memberTypes.Items.AddIfNotNull((GraphQLNamedType)ToType(type));

            return memberTypes;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLFieldType"/> into <see cref="GraphQLImplementsInterfaces"/>.
    /// </summary>
    public virtual GraphQLImplementsInterfaces? ToImplementsInterfaces(ICollection<GraphQLFieldType>? types)
    {
        if (types?.Count > 0)
        {
            var interfaces = new GraphQLImplementsInterfaces(new());
            foreach (var type in types)
                interfaces.Items.AddIfNotNull((GraphQLNamedType)ToType(type));

            return interfaces;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLField"/> into <see cref="GraphQLFieldsDefinition"/>.
    /// </summary>
    public virtual GraphQLFieldsDefinition? ToFieldsDefinition(ICollection<GraphQLField>? fields)
    {
        if (fields?.Count > 0)
        {
            var definitions = new GraphQLFieldsDefinition(new());
            foreach (var field in fields)
                definitions.Items.AddIfNotNull(ToFieldDefinition(field));

            return definitions;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLInputField"/> into <see cref="GraphQLInputFieldsDefinition"/>.
    /// </summary>
    public virtual GraphQLInputFieldsDefinition? ToInputFieldsDefinition(ICollection<GraphQLInputField>? fields)
    {
        if (fields?.Count > 0)
        {
            var definitions = new GraphQLInputFieldsDefinition(new());
            foreach (var field in fields)
                definitions.Items.Add(ToInputValueDefinition(field));

            return definitions;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLArgument"/> into <see cref="GraphQLArgumentsDefinition"/>.
    /// </summary>
    public virtual GraphQLArgumentsDefinition? ToArgumentsDefinition(ICollection<GraphQLArgument>? arguments)
    {
        if (arguments?.Count > 0)
        {
            var definitions = new GraphQLArgumentsDefinition(new());
            foreach (var argument in arguments)
                definitions.Items.Add(ToInputValueDefinition(argument));

            return definitions;
        }

        return null;
    }

    /// <summary>
    /// Converts any implementation of <see cref="IHasDirectives"/> into <see cref="GraphQLDirectives"/>.
    /// </summary>
    public virtual GraphQLDirectives? ToDirectives(IHasDirectives element)
    {
        // In case of applied directives (if the server supports them) @deprecated should be
        // always among AppliedDirectives because AppliedDirectives is non-null field.
        if (element.AppliedDirectives?.Count > 0)
        {
            if (_options.PrintAppliedDirectives)
            {
                var directives = new GraphQLDirectives(new());
                foreach (var applied in element.AppliedDirectives)
                    directives.Items.AddIfNotNull(ToDirective(applied));

                return directives;
            }
        }
        // else fall back to manually create AppliedDirectives from single @deprecated if exists
        else if (element is IDeprecatable deprecatable && deprecatable.DeprecationReason != null)
        {
            var directives = new GraphQLDirectives(new());
            directives.Items.AddIfNotNull(ToDirective(new GraphQLAppliedDirective
            {
                Name = "deprecated",
                Args = new List<GraphQLDirectiveArgument>
                {
                    new GraphQLDirectiveArgument
                    {
                        Name = "reason",
                        // need to escape because later will call ToValue
                        Value = Helpers.BuildEscapedString(deprecatable.DeprecationReason),
                    },
                },
            }));
            return directives;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="GraphQLDirectiveArgument"/> into <see cref="GraphQLArguments"/>.
    /// </summary>
    public virtual GraphQLArguments? ToArguments(ICollection<GraphQLDirectiveArgument>? arguments)
    {
        if (arguments?.Count > 0)
        {
            var args = new GraphQLArguments(new());
            foreach (var argument in arguments)
                args.Items.AddIfNotNull(ToArgument(argument));

            return args;
        }

        return null;
    }

    /// <summary>
    /// Converts collection of <see cref="DirectiveLocation"/> into <see cref="GraphQLDirectiveLocations"/>.
    /// </summary>
    public virtual GraphQLDirectiveLocations ToDirectiveLocations(ICollection<GraphQLDirectiveLocation>? locations)
    {
        var directiveLocations = new GraphQLDirectiveLocations(new());
        if (locations != null)
        {
            foreach (var location in locations)
                directiveLocations.Items.Add((DirectiveLocation)location); // should match
        }

        return directiveLocations;
    }

}
