using GraphQL.Types;

namespace GraphQL.IntrospectionModel.Tests.Introspection;

internal class Mutation : ObjectGraphType<Person>
{
    public Mutation()
    {
        Field<BooleanGraphType>("print")
            .Argument<PersonGraphType>("person", arg => arg.DeprecationReason = "argument deprecation feature")
            .Resolve(context =>
        {
            Console.WriteLine(context.Source);
            return true;
        });
    }
}

internal class PersonGraphType : InputObjectGraphType<Person>
{
    public PersonGraphType()
    {
        Field(x => x.Age);
        Field(x => x.Name, nullable: true).DeprecationReason("input field deprecation feature");
    }
}

internal class Person
{
    public string? Name { get; set; }

    public int Age { get; set; }

    public override string ToString() => $"{Name}: {Age}";
}
