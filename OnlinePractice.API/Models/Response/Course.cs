namespace OnlinePractice.API.Models.Response
{

    public class CourseList
    {
        public List<Courses> Courses { get; set; } = new();
    }
    public class Courses
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid ExamTypeID { get; set; }
    }
}
