using System.ComponentModel.DataAnnotations;
using OnlinePractice.API.Models.Enum;

namespace OnlinePractice.API.Models.DBModel
{
    public class Question : BaseModel
    {
        /// <summary>
        /// Id of the Question
        /// </summary>
        [Key]
        public int QuestionId { get; set; }

        /// <summary>
        /// TestQuestion
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Correct Answer of the TestQuestion
        /// </summary>
        public string? CorrectAnswer { get; set; }

        /// <summary>
        /// Option A
        /// </summary>
        public string? OptionA { get; set; }

        /// <summary>
        /// Option B
        /// </summary>
        public string? OptionB { get; set; }

        /// <summary>
        /// Option C
        /// </summary>
        public string? OptionC { get; set; }

        /// <summary>
        /// Option D
        /// </summary>
        public string? OptionD { get; set; }

        public QuestionType? QuestionType { get; set; }

        public Guid CourseId { get; set; }

        public Guid SubCourseId { get; set; }
    }
}
