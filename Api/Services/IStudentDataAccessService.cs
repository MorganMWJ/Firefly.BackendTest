using Domain.Model;
using LanguageExt.Common;

namespace Api.Services;

public interface IStudentDataAccessService
{
    public Task<Student> CreateStudentAsync(Student newStudent);

    public Task<Result<Student>> GetStudentByIdAsync(int id);
}
