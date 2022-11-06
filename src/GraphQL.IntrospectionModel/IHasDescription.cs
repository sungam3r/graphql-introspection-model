namespace GraphQL.IntrospectionModel;

/// <summary> Interface for parts of schema that have a description. </summary>
public interface IHasDescription
{
    /// <summary> Gets or sets the item description. </summary>
    string? Description { get; set; }
}
