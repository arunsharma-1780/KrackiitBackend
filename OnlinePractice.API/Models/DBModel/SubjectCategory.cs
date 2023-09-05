namespace OnlinePractice.API.Models.DBModel
{
    public class SubjectCategory : BaseModel
    {
        public Guid SubjectId { get; set; }
        public Guid SubCourseId { get; set; }
    }
}
