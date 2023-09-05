using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class GetStudentEbook :CurrentUser
    {
        public Guid InstituteId { get ; set; }  
        public Guid SubcourseId { get ; set; }
        public Guid SubjectId { get; set; }
      //  public Guid TopicId { get; set; }
        public LanguageFilter LanguageFilter { get; set; }

        public PriceWiseSort PriceWiseSort { get; set; }
        public PricingFilter PriceFilter { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }

    public class EbooksSubjects
    {
        public Guid InstituteId { get; set; }
        public Guid SubcourseId { get; set; }
    }

    public class GetEbookById :CurrentUser
    {
        public Guid Id
        { get; set; }
    }


}
