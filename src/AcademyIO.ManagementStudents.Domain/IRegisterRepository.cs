using AcademyIO.Core.Data;

namespace AcademyIO.ManagementStudents.Domain
{
    public interface IRegistrationRepository : IRepository<User>
    {
        Task<Registration> FinishCourse(Guid studentId, Guid courseId);
        Registration AddRegistration(Guid studentId, Guid courseId);
    }
}
