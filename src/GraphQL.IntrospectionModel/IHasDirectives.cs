using System.Collections.Generic;

namespace GraphQL.IntrospectionModel
{
    /// <summary> Interface for parts of schema that have directives. </summary>
    public interface IHasDirectives
    {
        /// <summary> Gets or sets the set of directives applied to the element. </summary>
        ICollection<GraphQLAppliedDirective> AppliedDirectives { get; set; }
    }
}
