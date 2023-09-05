namespace OnlinePractice.API.Models.Response
{
    public class Video
    {
        public Guid Id { get; set; }
        public Guid ExamTypeId { get; set; }

        public string ExamTypeName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public Guid SubCourseId { get; set; }

        public string SubCourseName { get; set; } = string.Empty;

        public Guid SubjectCategoryId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public Guid TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid InstituteId { get; set; }
        public string InstituteName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoThumbnail { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
        public int TotalPurchase { get; set; }

    }
    public class VideoList
    {
        public int TotalRecords { get; set; }
        public List<Videos> Videos { get; set; } = new();
    }
    public class VideoListV2
    {
        public List<VideoLists> Videos { get; set; } = new();
    }
    public class VideoLists
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class FacultyList
    {
        public List<Faculties> Faculties { get; set; } = new();
    }

    public class Faculties
    {
        public string FacultyName { get; set; } = string.Empty;
    }
    public class Videos
    {
        public Guid Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string VideoThumbnail { get; set; } = string.Empty;
        public DateTime? CreationDateTime { get; set; } = null;
        public Guid? CreatorUserId { get; set; } = null;
        public string Language { get; set; } = string.Empty;
        public double Price { get; set; }
    }
    public class YearList
    {
        public List<int> Years { get; set; } = new();
    }

    public class AuthorAndLanguage2
    {
        public string Language { get; set; } = string.Empty;
        public string authorName { get; set; } = string.Empty;
    }
}
