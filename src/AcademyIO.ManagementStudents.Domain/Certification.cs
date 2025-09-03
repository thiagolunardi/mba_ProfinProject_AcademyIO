using AcademyIO.Core.DomainObjects;
using AcademyIO.ManagementCourses.Domain;

namespace AcademyIO.ManagementStudents.Domain
{
    public class Certification : Entity
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public User Student { get; private set; }
    }
}
