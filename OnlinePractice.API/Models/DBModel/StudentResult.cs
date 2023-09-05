using OnlinePractice.API.Models.Response;
using Stripe;

namespace OnlinePractice.API.Models.DBModel
{
    public class StudentResult : BaseModel
    {
        public Guid StudentId {get;set;}
        public Guid MockTestId { get;set;}
        public Guid SubjectId { get;set;}
        public int CorrectAnswer { get;set;}
        public int InCorrectAnswer { get;set;}
        public int SkippedAnswer { get;set;}
        public double TotalMarks { get;set;}
        public int TotalQuestion { get;set;}
        public double ObtainMarks { get;set;}
        public Guid UniqueMockTetId { get;set;}
        public bool IsCustom { get;set;}

    }
}
