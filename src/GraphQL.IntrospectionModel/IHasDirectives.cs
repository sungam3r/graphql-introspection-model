using System.Collections.Generic;

namespace GraphQL.IntrospectionModel
{
    /// <summary> Interface for parts of schema that have directives. </summary>
    public interface IHasDirectives
    {
        /// <summary> Gets or sets the set of directives for the element. </summary>
        ICollection<GraphQLDirectiveUsage> Directives { get; set; }
    }
}
