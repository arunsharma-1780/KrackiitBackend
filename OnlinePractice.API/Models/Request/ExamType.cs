using OnlinePractice.API.Models.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.Request
{
    public class CreateExamFlow :CurrentUser
    {
        public string ExamTypeName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubCourseName { get; set; } = string.Empty; 
        public List<SubjectIds> SubjectIds { get; set; } = new ();
    }
    public class CreateExamType:CurrentUser
    {
        public string ExamName { get; set; }=string.Empty;
    }

    public class EditExamType : CreateExamType
    {
       public Guid Id { get; set; }
    }

    public class ExamTypeById : CurrentUser
    {
        public Guid Id { get; set; }
    }

    public class ExamName
    {
        public string Name { get; set; } = string.Empty;
    }
    public class SubjectIds
    {
        public Guid SubjectId { get; set; } 
    }

}
