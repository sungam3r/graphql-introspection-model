using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.IntrospectionModel.SDL;
using GraphQLParser.Visitors;

namespace GraphQL.IntrospectionModel.Tests;

public class ASTConverterTests
{
    [Fact]
    public void ASTConverter_ToTypeDefinition_Should_Throw_On_Unknown_TypeKind()
    {
        var converter = new ASTConverter();
        Should.Throw<NotSupportedException>(() => converter.ToTypeDefinition(new GraphQLType { Kind = (GraphQLTypeKind)12345 })).Message.ShouldBe("12345");
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1869:Cache and reuse 'JsonSerializerOptions' instances", Justification = "Test")]
    public void Should_Build_Schema_From_Introspection_Response()
    {
        string introspection = ReadFile("test1.json");
        var response = JsonSerializer.Deserialize<GraphQLResponse>(introspection, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        string sdl = response.ShouldNotBeNull().Data.ShouldNotBeNull().__Schema.ShouldNotBeNull().Print(new ASTConverterOptions { EachDirectiveLocationOnNewLine = true });
        sdl.ShouldBe(ReadFile("test1.graphql"));
    }

    [Fact]
    public void Should_Build_Person_Schema()
    {
        var schema = new GraphQLSchema
        {
            QueryType = new GraphQLRequestType
            {
                Name = "Person"
            },
            Types =
            [
                new GraphQLType
                {
                    Name = "Person",
                    Fields =
                    [
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
                    ]
                }
            ]
        };

        string sdl = schema.Print();

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
            Types =
            [
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
                    Interfaces =
                    [
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
                    ]
                }
            ]
        };

        string sdl = schema.Print();

        sdl.ShouldBe(ReadFile("interfaces.graphql"));
    }

    // https://github.com/graphql/graphql-spec/pull/805
    [Fact]
    public void Should_Build_Schema_With_New_Deprecations_From_Draft_Spec()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Name = "Query",
                    Kind = GraphQLTypeKind.OBJECT,
                    Fields =
                    [
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
                            Args =
                            [
                                new GraphQLArgument
                                {
                                    Name = "filter",
                                    Type = new GraphQLFieldType
                                    {
                                        Name = "PersonFilter",
                                    },
                                    AppliedDirectives =
                                    [
                                        new GraphQLAppliedDirective
                                        {
                                            Name = "deprecated",
                                            Args =
                                            [
                                                new GraphQLDirectiveArgument
                                                {
                                                    Name = "reason",
                                                    Value = "\"Do not use this arg\""
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                },
                new GraphQLType
                {
                    Name = "PersonFilter",
                    Kind = GraphQLTypeKind.INPUT_OBJECT,
                    InputFields =
                    [
                        new GraphQLInputField
                        {
                            Name = "namePattern",
                            Type = new GraphQLFieldType
                            {
                                Name = "String"
                            },
                            AppliedDirectives =
                            [
                                new GraphQLAppliedDirective
                                {
                                    Name = "deprecated",
                                    Args =
                                    [
                                        new GraphQLDirectiveArgument
                                        {
                                            Name = "reason",
                                            Value = "\"Do not use this field\""
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                },
                new GraphQLType
                {
                    Name = "Person",
                    Kind = GraphQLTypeKind.OBJECT,
                    Fields =
                    [
                        new GraphQLField
                        {
                            Name = "name",
                            Type = new GraphQLFieldType
                            {
                                Name = "String"
                            }
                        }
                    ]
                },
            ]
        };

        string sdl = schema.Print();

        sdl.ShouldBe(ReadFile("deprecations.graphql"));
    }

    [Fact]
    public void Should_Build_Scalars()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.SCALAR,
                    Name = "Xml",
                },
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.SCALAR,
                    Name = "JSON",
                    AppliedDirectives =
                    [
                        new GraphQLAppliedDirective
                        {
                            Name = "some"
                        }
                    ]
                }
            ]
        };

        string sdl = schema.Print(new ASTConverterOptions { TypeComparer = null });

        sdl.ShouldBe(ReadFile("scalars.graphql"));
    }

    [Fact]
    public void Should_Build_Directives()
    {
        var schema = new GraphQLSchema
        {
            Directives =
            [
                // invalid usage, but should not throw
                new GraphQLDirective
                {
                    Name = "without_location1",
                    Locations = null!,
                },
                // invalid usage, but should not throw
                new GraphQLDirective
                {
                    Name = "without_location2",
                    Locations = [],
                },
                new GraphQLDirective
                {
                    Name = "my",
                    Locations =
                    [
                        GraphQLDirectiveLocation.QUERY
                    ]
                },
                new GraphQLDirective
                {
                    Name = "your",
                    IsRepeatable = true,
                    Locations =
                    [
                        GraphQLDirectiveLocation.ENUM
                    ]
                },
                new GraphQLDirective
                {
                    Name = "my_with_args",
                    Locations =
                    [
                        GraphQLDirectiveLocation.QUERY
                    ],
                    Args =
                    [
                        new GraphQLArgument
                        {
                            Name = "if",
                            Type = new GraphQLFieldType
                            {
                                Name = "Boolean"
                            }
                        }
                    ]
                },
                new GraphQLDirective
                {
                    Name = "your_with_args",
                    IsRepeatable = true,
                    Locations =
                    [
                        GraphQLDirectiveLocation.ENUM
                    ],
                    Args =
                    [
                        new GraphQLArgument
                        {
                            Name = "if",
                            Type = new GraphQLFieldType
                            {
                                Name = "Boolean"
                            }
                        }
                    ]
                }
            ]
        };

        string sdl = schema.Print(new ASTConverterOptions
        {
            EachDirectiveLocationOnNewLine = true,
            DirectiveComparer = null,
        });

        sdl.ShouldBe(ReadFile("directives.graphql"));
    }

    [Fact]
    public void Should_Build_Empty_Schema()
    {
        var schema = new GraphQLSchema
        {
        };

        // also tests parameterless ctor
        var document = new ASTConverter().ToDocument(schema);
        string sdl = new SDLPrinter().Print(document);

        sdl.ShouldBe("");
    }

    [Fact]
    public void Should_Build_Empty_Schema_When_PrintAppliedDirectives_False()
    {
        var schema = new GraphQLSchema
        {
            AppliedDirectives =
            [
                new GraphQLAppliedDirective
                {
                    Name = "my"
                }
            ]
        };

        string sdl = schema.Print(new ASTConverterOptions { PrintAppliedDirectives = false });

        sdl.ShouldBe("");
    }

    [Fact]
    public void Should_Build_Schema_With_QueryType()
    {
        var schema = new GraphQLSchema
        {
            QueryType = new GraphQLRequestType
            {
                Name = "MyQuery"
            }
        };

        string sdl = schema.Print();

        sdl.ShouldBe("""
            schema {
              query: MyQuery
            }

            """);
    }

    [Fact]
    public void Should_Build_Schema_With_MutationType()
    {
        var schema = new GraphQLSchema
        {
            MutationType = new GraphQLRequestType
            {
                Name = "MyMutation"
            }
        };

        string sdl = schema.Print();

        sdl.ShouldBe("""
            schema {
              mutation: MyMutation
            }

            """);
    }

    [Fact]
    public void Should_Build_Schema_With_SubscriptionType()
    {
        var schema = new GraphQLSchema
        {
            SubscriptionType = new GraphQLRequestType
            {
                Name = "MySubscription"
            }
        };

        string sdl = schema.Print();

        sdl.ShouldBe("""
            schema {
              subscription: MySubscription
            }

            """);
    }

    [Fact]
    public void Should_Build_Enums()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.ENUM,
                    Name = "Color",
                    EnumValues =
                    [
                        new GraphQLEnumValue
                        {
                            Name = "RED"
                        },
                        new GraphQLEnumValue
                        {
                            Name = "GREEN",
                            IsDeprecated = true,
                            DeprecationReason = "Use RED",
                        },
                        new GraphQLEnumValue
                        {
                            Name = "BLUE",
                            AppliedDirectives =
                            [
                                new GraphQLAppliedDirective
                                {
                                    Name = "some"
                                }
                            ]
                        }
                    ]
                },
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.ENUM,
                    Name = "Status",
                    EnumValues =
                    [
                        new GraphQLEnumValue
                        {
                            Name = "Ok"
                        },
                        new GraphQLEnumValue
                        {
                            Name = "Error",
                        }
                    ],
                    AppliedDirectives =
                    [
                        new GraphQLAppliedDirective
                        {
                            Name = "v2"
                        }
                    ]
                }
            ]
        };

        string sdl1 = schema.Print();
        sdl1.ShouldBe(ReadFile("enums.graphql"));

        string sdl2 = schema.Print(new ASTConverterOptions { PrintAppliedDirectives = false });
        sdl2.ShouldBe(ReadFile("enums_without_directives.graphql"));
    }

    [Fact]
    public void Should_Build_Empty_Enum()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.ENUM,
                    Name = "Color",
                }
            ]
        };

        string sdl1 = schema.Print();
        sdl1.ShouldBe("""
            enum Color

            """);
    }

    [Fact]
    public void Should_Build_Empty_Union()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.UNION,
                    Name = "Reason",
                }
            ]
        };

        string sdl1 = schema.Print();
        sdl1.ShouldBe("""
            union Reason

            """);
    }

    [Fact]
    public void Should_Build_Empty_Input()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Kind = GraphQLTypeKind.INPUT_OBJECT,
                    Name = "Person",
                }
            ]
        };

        string sdl1 = schema.Print();
        sdl1.ShouldBe("""
            input Person

            """);
    }

    [Fact]
    public void Should_Build_Escaped_DeprecationReason()
    {
        var schema = new GraphQLSchema
        {
            Types =
            [
                new GraphQLType
                {
                    Name = "Query",
                    Fields =
                    [
                        new GraphQLField
                        {
                            Name = "field1",
                            DeprecationReason = "Reason that should be escaped: \", \\, \b, \f, \n, \r, \t \u0005 ", // as is
                            Type = new GraphQLFieldType
                            {
                               Name = "String"
                            }
                        },
                        new GraphQLField
                        {
                            Name = "field2",
                            Type = new GraphQLFieldType
                            {
                                Name = "String"
                            },
                            AppliedDirectives =
                            [
                                new GraphQLAppliedDirective
                                {
                                    Name = "deprecated",
                                    Args =
                                    [
                                        new GraphQLDirectiveArgument
                                        {
                                            Name = "reason",
                                            Value = "\"Reason that should be escaped: \\\", \\\\, \\b, \\f, \\n, \\r, \\t \\u0005 \"" // escaped string value
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        string sdl = schema.Print();

        sdl.ShouldBe(ReadFile("escaped.graphql"));
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Files", fileName));
}
