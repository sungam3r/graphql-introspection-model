using System.Collections.Generic;
using System.Diagnostics;

namespace GraphQL.IntrospectionModel
{
    /// <summary> GraphQL type. One can say that this is the type of the first level, and the second level is described by <see cref="GraphQLFieldType"/>. </summary>
    [DebuggerDisplay("{Name,nq}")]
    public sealed class GraphQLType : GraphQLTypeDescriptor, IHasDescription, IHasDirectives
    {
        /// <summary> Gets or sets the type description. </summary>
        public string Description { get; set; }

        /// <summary> Gets or sets the set of all type fields. Used for OBJECT. </summary>
        public ICollection<GraphQLField> Fields { get; set; }

        /// <summary> Gets or sets the set of all type fields. Used for INPUT_OBJECT. </summary>
        public ICollection<GraphQLInputField> InputFields { get; set; }

        /// <summary> Gets or sets the set of interfaces implemented by this type. Used for OBJECT. </summary>
        public ICollection<GraphQLFieldType> Interfaces { get; set; }

        /// <summary> Gets or sets the enumeration values. Used for ENUM. </summary>
        public ICollection<GraphQLEnumValue> EnumValues { get; set; }

        /// <summary> Gets or sets the possible values for the type. Used for UNION. </summary>
        public ICollection<GraphQLFieldType> PossibleTypes { get; set; }

        /// <summary> Gets or sets the set of directives for the type. </summary>
        public ICollection<GraphQLDirectiveUsage> Directives { get; set; }
    }
}
