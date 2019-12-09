using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> GraphQL field type. One can say that this is a type of the second level, and the first level is described by <see cref="GraphQLType"/>. </summary>
    [DebuggerDisplay("{SDLType,nq}")]
    public sealed class GraphQLFieldType : GraphQLTypeDescriptor
    {
        /// <summary> Gets or sets the internal type. For a field like [Int!]! the chain from the external to the internal will be: [Int!]! -> [Int!] -> Int! -> Int. </summary>
        public GraphQLFieldType OfType { get; set; }

        /// <summary> Gets the SDL type of the field. </summary>
        public string SDLType => Kind switch
        {
            GraphQLTypeKind.List     => $"[{OfType.SDLType}]",
            GraphQLTypeKind.Non_Null => OfType.SDLType + "!",
            _                        => Name
        };
    }
}
