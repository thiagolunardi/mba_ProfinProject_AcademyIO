using AcademyIO.ManagementCourses.Application.Queries.ViewModels;

namespace AcademyIO.ManagementCourses.Application.Queries
{
    public interface ICourseQuery
    {
        Task<IEnumerable<CourseViewModel>> GetAll();

        Task<CourseViewModel> GetById(Guid courseId);
    }
}
