using System.ComponentModel;

namespace OnlinePractice.API.Models.Request
{
    public class GeAdminResult : CurrentUser
    {
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
        public Guid MockTestId { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
    public class GeResultByMockTestId : CurrentUser
    {
        public Guid StudentId { get; set; }
        public Guid MockTestId { get; set; }
    }

    public class GeResultAnalysisDetail : CurrentUser
    {
        public Guid StudentId { get; set; }
    }

    public class GetMockTestList
    {
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
    }

}
