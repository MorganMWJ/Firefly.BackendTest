using Database;
using Domain.Model;
using FluentAssertions.Equivalency.Tracing;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Api.Tests.Component;

public class ControllerTestsFixture : IDisposable
{
    public TestApplicationFactory Factory { get; }
    public ApiContext DbContext { get; }
    public HttpClient Client { get; }
    public Faker Faker { get; }

    private readonly TeacherFaker _teacherFaker = new TeacherFaker();
    private readonly StudentFaker _studentFaker = new StudentFaker();
    private readonly ClassFaker _classFaker = new ClassFaker();

    public ControllerTestsFixture()
    {
        Factory = new TestApplicationFactory();
        Client = Factory.CreateClient();
        Faker = new Faker();
        DbContext = Factory.InMemoryContext;

        // Ensure database is created and seeded with data
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
        SeedDatabase(DbContext);
    }

    private void SeedDatabase(ApiContext dbContext)
    {
        // Check if data already exists to avoid duplication
        if (!dbContext.Classes.Any())
        {
            var seedClasses = _classFaker.Generate(3);
            dbContext.Classes.AddRange(seedClasses);
            dbContext.SaveChanges();
        }
        
        if (!dbContext.Teachers.Any())
        {
            var seedteachers = _teacherFaker.Generate(3);
            dbContext.Teachers.AddRange(seedteachers);
            dbContext.SaveChanges();
        }

        if (!dbContext.Students.Any())
        {
            var seedStudents = _studentFaker.Generate(6);
            dbContext.Students.AddRange(seedStudents);
            dbContext.SaveChanges();
        }
    }

    public void SeedTeacherWithMaxClasses()
    {
        var seedClasses = _classFaker
            .RuleFor(t => t.Name, f => $"CompSci {f.Random.AlphaNumeric(5)}")
            .Generate(5);
        DbContext.Classes.AddRange(seedClasses);
        DbContext.SaveChanges();

        var seedTeacher = _teacherFaker.Generate();
        DbContext.Teachers.AddRange(seedTeacher);
        DbContext.SaveChanges();

        var classesToAssign = DbContext.Classes.Where(c => c.Name.StartsWith("CompSci"));
        var teacherToAssign = DbContext.Teachers.Single(t => t.Id == seedTeacher.Id);

        foreach (Class cls in classesToAssign)
        {
            cls.Teacher = teacherToAssign;
            teacherToAssign.Classes.Add(cls);
            DbContext.SaveChanges();
        }
    }

    public void SeedOverCapacityClass(int classId)
    {        
        var overCapacityClass = DbContext.Classes.Single(c => c.Id == classId);

        // just set capacity to 0 for now
        overCapacityClass.Capacity = 0;

        DbContext.SaveChanges();
    }

    // Reset the database by clearing data after test
    public void Dispose()
    {
        DbContext.Classes.RemoveRange(DbContext.Classes);
        DbContext.Teachers.RemoveRange(DbContext.Teachers);
        DbContext.Students.RemoveRange(DbContext.Students);
        DbContext.SaveChanges();
        DbContext.Database.EnsureDeleted();
    }
}

[CollectionDefinition("ControllerTests")]
public class DatabaseCollection : ICollectionFixture<ControllerTestsFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
