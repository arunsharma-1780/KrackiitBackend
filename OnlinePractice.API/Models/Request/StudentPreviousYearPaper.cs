using OnlinePractice.API.Models.Enum;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class PYPInstitutes
    {
        public Guid InstituteId { get; set; }
        public Guid SubcourseId { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }

    }

    public class StudentPreviousYearPaperFilter : CurrentUser
    {
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
        public int? Year { get; set; }
        public LanguageFilter LanguageFilter { get; set; }
        public PriceWiseSort PriceWiseSort { get; set; }
        public PricingFilter PricingFilter { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        
    }
}
