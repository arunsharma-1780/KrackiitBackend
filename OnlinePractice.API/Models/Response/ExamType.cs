
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using DM = OnlinePractice.API.Models.DBModel;
namespace OnlinePractice.API.Models.Response
{
    public class ExamTypeList
    {
        public List<ExamType> ExamType { get; set; } = new();
    }
    public class ExamType
    {
        public Guid Id { get; set; }
        public string ExamName { get; set; } = string.Empty;

    }

    public class ExamList
    {
        public List<ExamTypeData> ExamType { get; set; } = new();
    }

    public class ExamTypeData
    {
        public Guid Id { get; set; }
        public string ExamName { get; set; } = string.Empty;

        public List<CoursesData> CoursesList { get; set; } = new();
    }
    public class CoursesData
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
    }
}