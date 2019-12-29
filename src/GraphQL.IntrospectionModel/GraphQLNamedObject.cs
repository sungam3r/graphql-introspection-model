﻿using System.Collections.Generic;

namespace GraphQL.IntrospectionModel
{
    /// <summary> The base class for the named GraphQL parts of the schema. </summary>
    public abstract class GraphQLNamedObject : IHasDescription, IHasDirectives
    {
        /// <summary> Gets or sets the name of the element. </summary>
        public string Name { get; set; }

        /// <summary> Gets or sets the description of the element. </summary>
        public string Description { get; set; }

        /// <summary> Gets or sets the set of directives for the element. </summary>
        public ICollection<GraphQLDirectiveUsage> Directives { get; set; }
    }
}