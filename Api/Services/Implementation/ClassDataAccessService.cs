using Database;
using Domain.Model;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementation;

public class ClassDataAccessService : IClassDataAccessService
{
    private readonly ApiContext _context;

    public ClassDataAccessService(ApiContext context)
    {
        _context = context;
    }

    public async Task<Result<Teacher>> AssignClassAsync(int teacherId, int classId)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

        if (cls == null)
        {
            var invalidOperationException = new InvalidOperationException($"Class doesn't exist with Id {classId}");
            return new Result<Teacher>(invalidOperationException);
        }

        var teacher = await _context.Teachers.Include(t => t.Classes).FirstOrDefaultAsync(c => c.Id == teacherId);

        if (teacher == null)
        {
            var invalidOperationException = new InvalidOperationException($"Teacher doesn't exist with Id {teacherId}");
            return new Result<Teacher>(invalidOperationException);
        }

        // indempotent if teacher is already assigned this class
        if (!teacher.Classes.Any(c => c.Id == classId)) 
        {
            if (teacher.Classes.Count() >= 5)
            {
                var validationException = new Exception("A teacher cannot be assigned to more that 5 classes.");
                return new Result<Teacher>(validationException);
            }

            cls.Teacher = teacher;
            teacher.Classes.Add(cls);

            await _context.SaveChangesAsync();
        }

        return teacher;
    }

    public async Task<Class> CreateClassAsync(Class newClass)
    {
        _context.Add(newClass);
        await _context.SaveChangesAsync();

        return newClass;
    }

    public async Task<Student> CreateStudentAsync(Student newStudent)
    {
        _context.Add(newStudent);
        await _context.SaveChangesAsync();

        return newStudent;
    }

    public async Task<Teacher> CreateTeacherAsync(Teacher newTeacher)
    {
        _context.Add(newTeacher);
        await _context.SaveChangesAsync();

        return newTeacher;
    }

    public async Task<Result<Student>> EnrollStudentAsync(int classId, int studentId)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

        if (cls == null)
        {
            var invalidOperationException = new InvalidOperationException($"Class doesn't exist with Id {classId}");
            return new Result<Student>(invalidOperationException);
        }

        var student = await _context.Students.FirstOrDefaultAsync(c => c.Id == studentId);

        if (student == null)
        {
            var invalidOperationException = new InvalidOperationException($"Student doesn't exist with Id {studentId}");
            return new Result<Student>(invalidOperationException);
        }

        if (cls.Students.Count >= cls.Capacity)
        {
            var validationException = new Exception("A student cannot be enrolled in a class that is over its capacity");
            return new Result<Student>(validationException);
        }

        cls.Students.Add(student);
        student.Classes.Add(cls);

        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<Result<Class>> GetClassByIdAsync(int id)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);

        if (cls == null)
        {
            var invalidOperationException = new InvalidOperationException($"Class doesn't exist with Id {id}");
            return new Result<Class>(invalidOperationException);
        }

        return cls;
    }

    public async Task<Result<Student>> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(c => c.Id == id);

        if (student == null)
        {
            var invalidOperationException = new InvalidOperationException($"Student doesn't exist with Id {id}");
            return new Result<Student>(invalidOperationException);
        }

        return student;
    }

    public async Task<Result<Teacher>> GetTeacherByIdAsync(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(c => c.Id == id);

        if (teacher == null)
        {
            var invalidOperationException = new InvalidOperationException($"Teacher doesn't exist with Id {id}");
            return new Result<Teacher>(invalidOperationException);
        }

        return teacher;
    }
}
