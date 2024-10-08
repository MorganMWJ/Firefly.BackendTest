using Domain.Model;

namespace Api.Tests;

public class StudentFaker : Faker<Student>
{
    public StudentFaker()
    {
        Randomizer.Seed = new Random(123);

        RuleFor(t => t.Id, f => f.IndexFaker + 1);
        RuleFor(t => t.Name, f => f.Name.FullName());
        RuleFor(t => t.Email, (f, t) => f.Internet.Email(t.Name));
    }
}
