using Domain.Model;
using LanguageExt.Common;

namespace Api.Services;

public interface ITeacherDataAccessService
{
    public Task<Teacher> CreateTeacherAsync(Teacher newTeacher);

    public Task<Result<Teacher>> GetTeacherByIdAsync(int id);

    public Task<Result<Teacher>> AssignClassAsync(int teacherId, int classId);
}
