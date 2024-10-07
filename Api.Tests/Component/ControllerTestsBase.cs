using Database;
using Domain.Model;
using FluentAssertions.Equivalency.Tracing;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Api.Tests.Component;

public class ControllerTestsBase : IDisposable
{
    protected readonly TestApplicationFactory _factory;
    protected readonly ApiContext _dbContext;
    protected readonly HttpClient _client;
    protected readonly Faker _faker;

    public ControllerTestsBase()
    {
        _factory = new TestApplicationFactory();
        _client = _factory.CreateClient();
        _faker = new Faker();
        _dbContext = _factory.InMemoryContext;

        // Ensure database is created and seeded with data
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        SeedDatabase(_dbContext);
    }

    public static void SeedDatabase(ApiContext dbContext)
    {
        // Check if data already exists to avoid duplication
        if (!dbContext.Classes.Any())
        {
            dbContext.Classes.AddRange(
                new Class { Id = 1, Name = "Math", Capacity = 25, Students = new Collection<Student>() },
                new Class { Id = 2, Name = "Science", Capacity = 35, Students = new Collection<Student>() },
                new Class { Id = 3, Name = "History", Capacity = 30, Students = new Collection<Student>() }
            );
            dbContext.SaveChanges(); // Commit the seeded data
        }

        if (!dbContext.Teachers.Any())
        {
            dbContext.Teachers.AddRange(
                new Teacher { Id = 1, Name = "Mrs E Carter-Evans", Email = "ece@teach.ac.uk", Classes = new Collection<Class>() },
                new Teacher { Id = 2, Name = "Mr A Jones", Email = "aj@teach.ac.uk", Classes = new Collection<Class>() },
                new Teacher { Id = 3, Name = "Mr B Yeoman", Email = "by@teach.ac.uk", Classes = new Collection<Class>() }
            );
            dbContext.SaveChanges();
        }

        if (!dbContext.Students.Any())
        {
            dbContext.Students.AddRange(
                new Student { Id = 1, Name = "Daniel Watkins", Email = "dw12@teach.ac.uk", Classes = new Collection<Class>() },
                new Student { Id = 2, Name = "Dylan Jones", Email = "dj52@teach.ac.uk", Classes = new Collection<Class>() },
                new Student { Id = 3, Name = "Joel Jones", Email = "jj87@teach.ac.uk", Classes = new Collection<Class>() },
                new Student { Id = 4, Name = "Ieuan Davies", Email = "id43@teach.ac.uk", Classes = new Collection<Class>() },
                new Student { Id = 5, Name = "Brandon Lodder", Email = "bd77@teach.ac.uk", Classes = new Collection<Class>() },
                new Student { Id = 6, Name = "Lewis Black", Email = "lb21@teach.ac.uk", Classes = new Collection<Class>() }
            );
            dbContext.SaveChanges();
        }
    }

    protected void SeedTeacherWithMaxClasses()
    {
        var classes = new Collection<Class>()
        {
            new Class { Name = "CompSci 1", Capacity = 25 },
            new Class { Name = "CompSci 2", Capacity = 35 },
            new Class { Name = "CompSci 3", Capacity = 30 },
            new Class { Name = "CompSci 4", Capacity = 30 },
            new Class { Name = "CompSci 5", Capacity = 30 }
        };
        _dbContext.Classes.AddRange(classes);
        _dbContext.SaveChanges();

        _dbContext.Teachers.AddRange(new Teacher { Id = 4, Name = "Mr Ross Evans", Email = "rosse@teach.ac.uk"});
        _dbContext.SaveChanges();

        var classesToAssign = _dbContext.Classes.Where(c => c.Name.StartsWith("CompSci"));
        var teacherToAssign = _dbContext.Teachers.Single(t => t.Id == 4);
        
        foreach(Class cls in classesToAssign)
        {
            cls.Teacher = teacherToAssign;
            teacherToAssign.Classes.Add(cls);
            _dbContext.SaveChanges();
        }
    }

    protected void SeedOverCapacityClass(int classId)
    {        
        var overCapacityClass = _dbContext.Classes.Single(c => c.Id == classId);

        // just set capacity to 0 for now
        overCapacityClass.Capacity = 0;

        _dbContext.SaveChanges();
    }

    // Reset the database by clearing data after test
    public void Dispose()
    {
        _dbContext.Classes.RemoveRange(_dbContext.Classes);
        _dbContext.Teachers.RemoveRange(_dbContext.Teachers);
        _dbContext.Students.RemoveRange(_dbContext.Students);
        _dbContext.SaveChanges();
        _dbContext.Database.EnsureDeleted();
    }
}
