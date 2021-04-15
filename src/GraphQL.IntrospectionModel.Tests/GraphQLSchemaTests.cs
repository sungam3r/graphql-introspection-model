using System;
using System.Collections.Generic;
using System.IO;
using GraphQL.IntrospectionModel.SDL;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace GraphQL.IntrospectionModel.Tests
{
    /// <summary>
    /// Basic tests for <see cref="GraphQLSchema"/>.
    /// </summary>
    public class GraphQLSchemaTests
    {
        /// <summary>
        /// SDLBuilder should build schema from introspection response.
        /// </summary>
        [Fact]
        public void SDLBuilder_Should_Build_Schema_From_Introspection()
        {
            string introspection = Read("test1.json");
            var schemaJson = JObject.Parse(introspection).Property("__schema").Value;
            var schema = schemaJson.ToObject<GraphQLSchema>();
            string sdl = SDLBuilder.Build(schema);
            sdl.ShouldBe(Read("test1.graphql"));
        }

        /// <summary>
        /// SDLBuilder should build simple schema.
        /// </summary>
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
