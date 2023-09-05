namespace OnlinePractice.API.Models.Enum
{
    public enum FormatType
    {
        PDF = 1,
        JPEG = 2,
        TEXT = 3,
        CVG = 4
    }

    public enum QuestionType
    {
        SingleChoice = 1,
        MCQ = 2,
        IntegerType = 3,
        MatchTheColumn = 4,
        Phrases = 5,

    }
    public enum QuestionLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public enum QuestionLanguage
    {
        English = 1,
        Hindi = 2,
        Gujarati = 3,
        Marathi = 4,
    }
}
