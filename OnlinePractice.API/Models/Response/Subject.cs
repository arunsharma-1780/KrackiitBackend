namespace OnlinePractice.API.Models.Response
{
    public class SubjectList
    {
        public int TotalRecords { get; set; }
        public List<Subject> Subjects { get; set; } = new();
    }
    public class Subject
    {
        public Guid Id { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }



    public class SubjectCategoryList
    {
        public List<SubjectCategory> SubjectCategories { get; set; } = new();
    }
    public class SubjectCategory
    {
        public string SubjectName { get; set; } = string.Empty;
        public Guid SubjectCategoryId { get; set; }
    }

}
