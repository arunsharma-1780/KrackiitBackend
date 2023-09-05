using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Response
{
    public class VideosSubjectsList
    {
        public List<VideosSubjects> videoSubjects { get; set; } = new();
    }
    public class VideosSubjects
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }
    public class StudentVideoList
    {
        public List<StudentVideoData> studentVideoDatas { get; set; } = new ();
        public int TotalRecords { get; set; }
    }
    public class StudentVideoData
    {
        public Guid VideoId { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public double Price { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string VideoThumbnail { get; set; } = string.Empty;
        public string VideoURL { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        [DefaultValue(false)]
        public bool IsPurchased { get; set; }
        public DateTime? CreationDate { get; set; }
    }
    public class GetVideo
    {
        public Guid VideoId { get; set; }
        public string VideoThumbnail { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public double Price { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoURL { get; set; } = string.Empty;
        public bool IsPurchased { get; set; }

    }

}
