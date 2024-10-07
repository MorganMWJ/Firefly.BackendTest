using Domain.Model;
using LanguageExt.Common;

namespace Api.Services;

public interface IClassDataAccessService
{
    public Task<Class> CreateClassAsync(Class newClass);

    public Task<Result<Class>> GetClassByIdAsync(int id);

    public Task<Student> CreateStudentAsync(Student newStudent);

    public Task<Result<Student>> GetStudentByIdAsync(int id);    

    public Task<Teacher> CreateTeacherAsync(Teacher newTeacher);

    public Task<Result<Teacher>> GetTeacherByIdAsync(int id);

    public Task<Result<Teacher>> AssignClassAsync(int teacherId, int classId);

    public Task<Result<Student>> EnrollStudentAsync(int classId, int studentId);
}
