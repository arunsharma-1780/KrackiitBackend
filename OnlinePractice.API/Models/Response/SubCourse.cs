namespace OnlinePractice.API.Models.Response
{
    public class SubCourseList
    {
        public List<SubCourse> SubCourses { get; set; } = new();
    }
    public class SubCourse
    {
        public Guid Id { get; set; }   
        public string SubCourseName { get; set; } = string.Empty;
        public Guid CourseID { get; set; }
    }

    public class SubCourseDataList
    {
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<SubCourses>? SubCourses { get; set; } = new();
    }
    public class SubCourses
    {
        public Guid Id { get; set; }
        public string SubCourseName { get; set; } = string.Empty;
   
    }
}
