using Database;
using Domain.Model;
using FluentAssertions.Equivalency.Tracing;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Api.Tests.Component;

public class ControllerTestsFixture : IDisposable
{
    public TestApplicationFactory Factory { get; }
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

        // Ensure database is created and seeded with data
        DbContextAccess(cxt =>
        {
            cxt.Database.EnsureDeleted();
            cxt.Database.EnsureCreated();
            SeedDatabase(cxt);
        });        
    }

    public async Task DbContextAccessAsync(Func<ApiContext, Task> action)
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var scopedServiceProvider = scope.ServiceProvider;
            var cxt = scopedServiceProvider.GetRequiredService<ApiContext>();

            await action(cxt);
        }
    }

    public void DbContextAccess(Action<ApiContext> action)
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var scopedServiceProvider = scope.ServiceProvider;
            var cxt = scopedServiceProvider.GetRequiredService<ApiContext>();

            action(cxt);
        }
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

    public void SeedTeacherWithMaxClasses(ApiContext dbContext)
    {
        var seedClasses = _classFaker
            .RuleFor(t => t.Name, f => $"CompSci {f.Random.AlphaNumeric(5)}")
            .Generate(5);
        dbContext.Classes.AddRange(seedClasses);
        dbContext.SaveChanges();

        var seedTeacher = _teacherFaker.Generate();
        dbContext.Teachers.AddRange(seedTeacher);
        dbContext.SaveChanges();

        var classesToAssign = dbContext.Classes.Where(c => c.Name.StartsWith("CompSci"));
        var teacherToAssign = dbContext.Teachers.Single(t => t.Id == seedTeacher.Id);

        foreach (Class cls in classesToAssign)
        {
            cls.Teacher = teacherToAssign;
            teacherToAssign.Classes.Add(cls);
            dbContext.SaveChanges();
        }
    }

    public void SeedOverCapacityClass(int classId)
    {
        DbContextAccess(cxt =>
        {
            var overCapacityClass = cxt.Classes.Single(c => c.Id == classId);

            // just set capacity to 0 for now
            overCapacityClass.Capacity = 0;

            cxt.SaveChanges();
        });
    }

    // Reset the database by clearing data after test
    public void Dispose()
    {
        DbContextAccess(cxt =>
        {
            cxt.Classes.RemoveRange(cxt.Classes);
            cxt.Teachers.RemoveRange(cxt.Teachers);
            cxt.Students.RemoveRange(cxt.Students);
            cxt.SaveChanges();
            cxt.Database.EnsureDeleted();
        });
    }
}

[CollectionDefinition("ControllerTests")]
public class DatabaseCollection : ICollectionFixture<ControllerTestsFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
