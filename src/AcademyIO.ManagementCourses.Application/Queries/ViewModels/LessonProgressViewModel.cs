using AcademyIO.Core.Enums;

namespace AcademyIO.ManagementCourses.Application.Queries
{
    public class LessonProgressViewModel(string lessonName, string progressLesson)
    {
        public string LessonName { get; } = lessonName;
        public string ProgressLesson { get; } = progressLesson;
    }
}