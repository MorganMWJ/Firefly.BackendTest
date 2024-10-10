using Domain.Model;

namespace Api.Tests;

public class ClassFaker : Faker<Class>
{
    public ClassFaker()
    {
        Randomizer.Seed = new Random(123);

        RuleFor(t => t.Name, f => f.Random.Word());
        RuleFor(t => t.Capacity, f => f.Random.Number(10, 50));
    }
}
