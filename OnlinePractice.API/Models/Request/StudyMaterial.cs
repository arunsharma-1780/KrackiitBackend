using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.Request
{
    public class CreateStudyMaterial:CurrentUser
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormatType FormatType { get; set; }
        public double Price { get; set; }
        public Guid CourseId { get; set; }
        public Guid ExamId { get; set; }
    }
    public class EditStudyMaterial : CreateStudyMaterial
    {
        public Guid Id { get; set; }
    }
    public class EditPriceStudyMaterial : CurrentUser
    {
        public Guid Id { get; set; }
        public double Price { get; set; }
    }
    public class MaterialById : CurrentUser
    {
        public Guid Id { get; set; }

    }
}
