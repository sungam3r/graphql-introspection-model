using GraphQL.IntrospectionModel.SDL;
using Newtonsoft.Json.Linq;
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
        var schemaJson = JObject.Parse(introspection).Property("__schema")!.Value;
        var schema = schemaJson.ToObject<GraphQLSchema>();
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
                                Kind = GraphQLTypeKind.Non_Null,
                                OfType = new GraphQLFieldType
                                {
                                    Kind = GraphQLTypeKind.Scalar,
                                    Name = "Int"
                                }
                            }
                        },
                        new GraphQLField
                        {
                            Name = "Name",
                            Type = new GraphQLFieldType
                            {
                                Kind = GraphQLTypeKind.Scalar,
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
                    Kind = GraphQLTypeKind.Interface
                },
                new GraphQLType
                {
                    Name = "IPerson2",
                    Kind = GraphQLTypeKind.Interface
                },
                new GraphQLType
                {
                    Name = "IPerson3",
                    Kind = GraphQLTypeKind.Interface
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
                    Kind = GraphQLTypeKind.Object,
                    Fields = new List<GraphQLField>
                    {
                        new GraphQLField
                        {
                            Name = "persons",
                            Type = new GraphQLFieldType
                            {
                                Kind = GraphQLTypeKind.List,
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
                    Kind = GraphQLTypeKind.Input_Object,
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
                    Kind = GraphQLTypeKind.Object,
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
    public void Should_Build_Schema_With_Directives_Only()
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
                }
            }
        };

        string sdl = SDLBuilder.Build(schema);

        sdl.ShouldBe(ReadFile("directives_only.graphql"));
    }

    private static string ReadFile(string fileName)
        => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", fileName));
}
