using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class QuestionBank :BaseModel
    {
        public Guid SubjectCategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public string QuestionRefId { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }  //MCQ =1,SingleChoice = 2,IntegerType =3,
        public QuestionLevel QuestionLevel { get; set; }// Easy=1,Medium=2,Hard=3
        public string QuestionLanguage { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionImageUrl { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionAImageUrl { get; set; } = string.Empty;
        public bool IsCorrectA { get; set; }
        public string OptionB { get; set; } = string.Empty;
        public string OptionBImageUrl { get; set; } = string.Empty;
        public bool IsCorrectB { get; set; }
        public string OptionC { get; set; } = string.Empty;
        public string OptionCImageUrl { get; set; } = string.Empty;
        public bool IsCorrectC { get; set; }
        public string OptionD { get; set; } = string.Empty;
        public string OptionDImageUrl { get; set; } = string.Empty;
        public bool IsCorrectD { get; set; }
        public int Mark { get; set; }
        public int NegativeMark { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public bool IsPartiallyCorrect { get; set; }
        public int PartialThreeCorrectMark { get; set; }
        public int PartialTwoCorrectMark { get; set; }
        public int PartialOneCorrectMark { get; set; }
    }
}
