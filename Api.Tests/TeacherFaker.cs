using Domain.Model;

namespace Api.Tests;

public class TeacherFaker : Faker<Teacher>
{
    public TeacherFaker()
    {
        Randomizer.Seed = new Random(123);

        RuleFor(t => t.Name, f => f.Name.FullName());
        RuleFor(t => t.Email, (f, t) => f.Internet.Email(t.Name));
    }
}
