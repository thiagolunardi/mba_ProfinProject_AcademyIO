using AcademyIO.Core.Data;
using AcademyIO.ManagementCourses.Domain;

namespace AcademyIO.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAll();

        Task<Course> GetById(Guid courseId);

        void Add(Course course);

        bool CourseExists(Guid courseI);
    }
}
