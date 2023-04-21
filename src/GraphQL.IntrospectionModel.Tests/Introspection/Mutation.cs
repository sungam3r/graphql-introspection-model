using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

internal sealed class Mutation : ObjectGraphType
{
    public Mutation()
    {
        Field<BooleanGraphType>("print")
            .Argument<PersonGraphType>("person")
            .Argument<StringGraphType>("format", arg => arg.DeprecationReason = "Unused argument")
            .Resolve(context =>
        {
            var person = context.GetArgument<Person>("person");
            Console.WriteLine(person);
            return true;
        });
    }
}

internal sealed class PersonGraphType : InputObjectGraphType<Person>
{
    public PersonGraphType()
    {
        Field(x => x.Name);
        Field(x => x.Age);
        Field(x => x.IsDeveloper, nullable: true).DeprecationReason("Use job title instead");
        Field(x => x.IsManager, nullable: true).DeprecationReason("Use job title instead");
        Field(x => x.JobTitle);
    }
}

internal sealed class Person
{
    public string? Name { get; set; }

    public int Age { get; set; }

    public bool IsDeveloper { get; set; }

    public bool IsManager { get; set; }

    public string? JobTitle { get; set; }

    public override string ToString() => $"{Name}: {Age}";
}
