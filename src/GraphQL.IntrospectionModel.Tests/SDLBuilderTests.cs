using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.IntrospectionModel.SDL;
using Shouldly;
using Xunit;

namespace GraphQL.IntrospectionModel.Tests;

public class SDLBuilderTests
{
    /// <summary>
    /// SDLBuilder should build schema from introspection response.
    /// </summary>
    [Fact]
    public void Should_Build_Schema_From_Introspection()
    {
        string introspection = ReadFile("test1.json");
        var schemaElement = JsonDocument.Parse(introspection).RootElement.GetProperty("__schema");
        var schema = JsonSerializer.Deserialize<GraphQLSchema>(schemaElement, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
        string sdl = SDLBuilder.Build(schema!);
        sdl.ShouldBe(ReadFile("test1.graphql"));
    }

    /// <summary>
    /// SDLBuilder should build simple schema.
    /// </summary>
    [Fact]
    public void Should_Build_Person_Schema()
    {
        var schema = new GraphQLSchema
        {
            QueryType = new GraphQLRequestType
            {
                Name = "Person"
            },
            Types = new List<GraphQLType>
            {
                new GraphQLType
                {
                    Name = "Person",
                    Fields = new List<GraphQLField>
                    {
                        new GraphQLField
                        {
                            Name = "Age",
                            Type = new GraphQLFieldType
                            {
                                Kind = GraphQLTypeKind.NON_NULL,
                                OfType = new GraphQLFieldType
                                {
                                    Kind = GraphQLTypeKind.SCALAR,
                                    Name = "Int"
                                }
                            }
                        },
                        new GraphQLField
                        {
                            Name = "Name",
                            Type = new GraphQLFieldType
                            {
                                Kind = GraphQLTypeKind.SCALAR,
                                Name = "String"
                            }
                        }
                    }
                }
            }
        };

        string sdl = SDLBuilder.Build(schema);

        sdl.ShouldBe(ReadFile("person.graphql"));
    }

    [Fact]
    public void Should_Build_Schema_With_Multiple_Interfaces()
    {
        var schema = new GraphQLSchema
        {
            QueryType = new GraphQLRequestType
            {
                Name = "Person"
            },
            Types = new List<GraphQLType>
            {
                new GraphQLType
                {
                    Name = "IPerson1",
                    Kind = GraphQLTypeKind.INTERFACE
                },
                new GraphQLType
                {
                    Name = "IPerson2",
                    Kind = GraphQLTypeKind.INTERFACE
                },
                new GraphQLType
                {
                    Name = "IPerson3",
                    Kind = GraphQLTypeKind.INTERFACE
                },
                new GraphQLType
                {
                    Name = "Person",
                    Interfaces = new List<GraphQLFieldType>
                    {
                        new GraphQLFieldType
                        {
                            Name = "IPerson1"
                        },
                        new GraphQLFieldType
                        {
                            Name = "IPerson2"
                        },
                        new GraphQLFieldType
                        {
                            Name = "IPerson3"
                        },
                    }
                }
            }
        };

        string sdl = SDLBuilder.Build(schema);

        sdl.ShouldBe(ReadFile("interfaces.graphql"));
    }

    // https://github.com/graphql/graphql-spec/pull/805
    [Fact]
    public void Should_Build_Schema_With_New_Deprecations_From_Draft_Spec()
    {
        var schema = new GraphQLSchema
        {
            Types = new List<GraphQLType>
            {
                new GraphQLType
                {
                    Name = "Query",
                    Kind = GraphQLTypeKind.OBJECT,
                    Fields = new List<GraphQLField>
                    {
                        new GraphQLField
                        {
                            Name = "persons",
                            Type = new GraphQLFieldType
                            {
                                Kind = GraphQLTypeKind.LIST,
                                OfType = new GraphQLFieldType
                                {
                                    Name = "Person"
                                }
                            },
                            Args = new List<GraphQLArgument>
                            {
                                new GraphQLArgument
                                {
                                    Name = "filter",
                                    Type = new GraphQLFieldType
                                    {
                                        Name = "PersonFilter",
                                    },
                                    AppliedDirectives = new List<GraphQLAppliedDirective>
                                    {
                                        new GraphQLAppliedDirective
                                        {
                                            Name = "deprecated",
                                            Args = new List<GraphQLDirectiveArgument>
                                            {
                                                new GraphQLDirectiveArgument
                                                {
                                                    Name = "reason",
                                                    Value = "\"Do not use this arg\""
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new GraphQLType
                {
                    Name = "PersonFilter",
                    Kind = GraphQLTypeKind.INPUT_OBJECT,
                    InputFields = new List<GraphQLInputField>
                    {
                        new GraphQLInputField
                        {
                            Name = "namePattern",
                            Type = new GraphQLFieldType
                            {
                                Name = "String"
                            },
                            AppliedDirectives = new List<GraphQLAppliedDirective>
                            {
                                new GraphQLAppliedDirective
                                {
                                    Name = "deprecated",
                                    Args = new List<GraphQLDirectiveArgument>
                                    {
                                        new GraphQLDirectiveArgument
                                        {
                                            Name = "reason",
                                            Value = "\"Do not use this field\""
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new GraphQLType
                {
                    Name = "Person",
                    Kind = GraphQLTypeKind.OBJECT,
                    Fields = new List<GraphQLField>
                    {
                        new GraphQLField
                        {
                            Name = "name",
                            Type = new GraphQLFieldType
                            {
                                Name = "String"
                            }
                        }
                    }
                },
            }
        };

        string sdl = SDLBuilder.Build(schema);

        sdl.ShouldBe(ReadFile("deprecations.graphql"));
    }

    [Fact]
    public void Should_Build_Directives()
    {
        var schema = new GraphQLSchema
        {
            Directives = new List<GraphQLDirective>
            {
                new GraphQLDirective
                {
                    Name = "my",
                    Locations = new List<GraphQLDirectiveLocation>
                    {
                        GraphQLDirectiveLocation.Query
                    }
                },
                new GraphQLDirective
                {
                    Name = "your",
                    IsRepeatable = true,
                    Locations = new List<GraphQLDirectiveLocation>
                    {
                        GraphQLDirectiveLocation.Enum
                    }
                },
                new GraphQLDirective
                {
                    Name = "my_with_args",
                    Locations = new List<GraphQLDirectiveLocation>
                    {
                        GraphQLDirectiveLocation.Query
                    },
                    Args = new List<GraphQLArgument>
                    {
                        new GraphQLArgument
                        {
                            Name = "if",
                            Type = new GraphQLFieldType
                            {
                                Name = "Boolean"
                            }
                        }
                    }
                },
                new GraphQLDirective
                {
                    Name = "your_with_args",
                    IsRepeatable = true,
                    Locations = new List<GraphQLDirectiveLocation>
                    {
                        GraphQLDirectiveLocation.Enum
                    },
                    Args = new List<GraphQLArgument>
                    {
                        new GraphQLArgument
                        {
                            Name = "if",
                            Type = new GraphQLFieldType
                            {
                                Name = "Boolean"
                            }
                        }
                    }
                }
            }
        };

        string sdl = SDLBuilder.Build(schema, new SDLBuilderOptions { DirectiveComparer = null });

        sdl.ShouldBe(ReadFile("directives.graphql"));
    }

    [Fact]
    public void Should_Build_Enums()
    {
        var schema = new GraphQLSchema
        {
            Types = new List<GraphQLType>
            {
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.ENUM,
                    Name = "Color",
                    EnumValues = new List<GraphQLEnumValue>
                    {
                        new GraphQLEnumValue
                        {
                            Name = "RED"
                        },
                        new GraphQLEnumValue
                        {
                            Name = "GREEN",
                            IsDeprecated = true,
                            DeprecationReason = "Use RED"
                        },
                        new GraphQLEnumValue
                        {
                            Name = "BLUE",
                            AppliedDirectives = new List<GraphQLAppliedDirective>
                            {
                                new GraphQLAppliedDirective
                                {
                                    Name = "some"
                                }
                            }
                        }
                    }
                },
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.ENUM,
                    Name = "Status",
                    EnumValues = new List<GraphQLEnumValue>
                    {
                        new GraphQLEnumValue
                        {
                            Name = "Ok"
                        },
                        new GraphQLEnumValue
                        {
                            Name = "Error",
                        }
                    },
                    AppliedDirectives = new List<GraphQLAppliedDirective>
                    {
                        new GraphQLAppliedDirective
                        {
                            Name = "v2"
                        }
                    }
                }
            }
        };

        string sdl = SDLBuilder.Build(schema);

        sdl.ShouldBe(ReadFile("enums.graphql"));
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", fileName));
}
