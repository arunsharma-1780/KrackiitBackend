namespace OnlinePractice.API.Models.Request
{
    public class CreateCourse:CurrentUser
    {
        public string CourseName { get; set; } = string.Empty;
        public Guid ExamTypeId { get; set; }
    }

    public class EditCourse : CreateCourse
    {
        public Guid Id { get; set; }
    }

    public class CourseById : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class GetExamId
    {
        public Guid ExamId { get; set; }
    }

    public class CourseName
    {
        public string Name { get; set; } = string.Empty; 
        public Guid ExamId { get; set; }
    }

}
