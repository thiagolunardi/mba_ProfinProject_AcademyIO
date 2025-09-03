using AcademyIO.Core.Enums;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;

namespace AcademyIO.ManagementCourses.Application.Queries
{
    public interface ILessonQuery
    {
        Task<IEnumerable<LessonViewModel>> GetAll();
        Task<IEnumerable<LessonViewModel>> GetByCourseId(Guid courseId);
        Task<IEnumerable<LessonProgressViewModel>> GetProgress(Guid studentId);
        EProgressLesson GetProgressStatusLesson(Guid lessonId, Guid studentId);
        bool ExistsProgress(Guid lessonId, Guid studentId);
    }
}
