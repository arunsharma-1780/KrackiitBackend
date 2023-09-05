namespace OnlinePractice.API.Models.Request
{

    public class CreateSubCourse : CurrentUser
    {
        public string SubCourseName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
    }

    public class EditSubCourse : CreateSubCourse
    {
        public Guid Id { get; set; }
    }

    public class SubCourseById : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class EditMultipleSubCourse : CurrentUser
    {
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<SubCourses> SubCourses { get; set; } = new();
    }
    public class SubCourses
    {
        public Guid Id { get; set; }
        public string SubCourseName { get; set; } = string.Empty;

    }

    public class SubCourseName
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
