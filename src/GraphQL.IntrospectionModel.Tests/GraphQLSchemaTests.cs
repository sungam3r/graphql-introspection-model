using GraphQL.IntrospectionModel.SDL;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace GraphQL.IntrospectionModel.Tests
{
    public class GraphQLSchemaTests
    {
        [Fact]
        public void Person_Schema_Should_Be_Built()
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

            var sdl = SDLBuilder.Build(schema);

            sdl.ShouldBe(Read("person.graphql"));
        }

        private string Read(string fileName)
        {
            return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
        }
    }
}
