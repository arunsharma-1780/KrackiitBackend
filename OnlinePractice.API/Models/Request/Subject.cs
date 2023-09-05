using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlinePractice.API.Models.Request
{

    public class CreateSubject : CurrentUser
    {
        public string SubjectName { get; set; } = string.Empty;
    }
    public class CreateSubjectCategory : CurrentUser
    {
        public Guid SubCourseId { get; set; }
        public List<SubjectById> SubjectIds { get; set; } = new();
  
    }
    public class EditSubjectCategory : CurrentUser
    {
        public Guid SubCourseId { get; set; }
        public List<EditSubjectId> SubjectIds { get; set; } = new();

    }

    public class EditSubject : CreateSubject
    {
        public Guid Id { get; set; }
    }

    public class SubjectById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class EditSubjectId : CurrentUser
    {
        public Guid Id { get; set; }
        public bool Value { get; set; }

    }

    public class SubjectCategoryById : CurrentUser
    {
        public Guid Id { get; set; }
    }
    public class GetSubject
    {
        [Required]
        public Guid SubCourseId { get; set; }
    }

    public class SubjectName
    {
        public Guid SubCourseId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class GetSubjects : CurrentUser
    {
        public Guid SubCourseId { get; set; }
    }

    public class GetAllSubject : CurrentUser
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

}
