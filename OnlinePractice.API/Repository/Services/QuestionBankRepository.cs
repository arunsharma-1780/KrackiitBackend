using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Models.Enum;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.Response;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Repository.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services
{
    public class QuestionBankRepository : IQuestionBankRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionBankRepository(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// CreateQuestionBank Method
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        public async Task<bool> CreateQuestionBank(Req.CreateQuestionBank questionBank)
        {
            if (questionBank != null)
            {
                int result = 0;
                if (questionBank.QuestionTableData.English != null)
                {
                    DM.QuestionBank question = new();
                    question.SubjectCategoryId = questionBank.SubjectCategoryId;
                    question.TopicId = questionBank.TopicId;
                    question.SubTopicId = questionBank.SubTopicId;
                    question.QuestionLanguage = QuestionLanguage.English.ToString();
                    question.QuestionType = questionBank.QuestionType;
                    question.QuestionLevel = questionBank.QuestionLevel;
                    question.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                    question.QuestionText = questionBank.QuestionTableData.English.QuestionText;
                    question.OptionA = questionBank.QuestionTableData.English.OptionA;
                    question.IsCorrectA = questionBank.QuestionTableData.English.IsCorrectA;
                    question.OptionB = questionBank.QuestionTableData.English.OptionB;
                    question.IsCorrectB = questionBank.QuestionTableData.English.IsCorrectB;
                    question.OptionC = questionBank.QuestionTableData.English.OptionC;
                    question.IsCorrectC = questionBank.QuestionTableData.English.IsCorrectC;
                    question.OptionD = questionBank.QuestionTableData.English.OptionD;
                    question.IsCorrectD = questionBank.QuestionTableData.English.IsCorrectD;
                    question.Explanation = questionBank.QuestionTableData.English.Explanation;
                    if (questionBank.QuestionType == QuestionType.SingleChoice)
                    {
                        question.IsPartiallyCorrect = false;
                        question.PartialThreeCorrectMark = 0;
                        question.PartialTwoCorrectMark = 0;
                        question.PartialOneCorrectMark = 0;
                    }
                    question.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                    question.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                    question.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                    question.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                    question.Mark = questionBank.Mark;
                    question.NegativeMark = questionBank.NegativeMark;
                    question.IsDeleted = false;
                    question.IsActive = true;
                    question.CreationDate = DateTime.UtcNow;
                    question.CreatorUserId = questionBank.UserId;
                    result = await _unitOfWork.Repository<DM.QuestionBank>().Insert(question);
                }
                if (questionBank.QuestionTableData.Hindi != null)
                {
                    DM.QuestionBank question = new();
                    question.SubjectCategoryId = questionBank.SubjectCategoryId;
                    question.TopicId = questionBank.TopicId;
                    question.SubTopicId = questionBank.SubTopicId;
                    question.QuestionLanguage = QuestionLanguage.Hindi.ToString();
                    question.QuestionType = questionBank.QuestionType;
                    question.QuestionLevel = questionBank.QuestionLevel;
                    question.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                    question.QuestionText = questionBank.QuestionTableData.Hindi.QuestionText;
                    question.OptionA = questionBank.QuestionTableData.Hindi.OptionA;
                    question.IsCorrectA = questionBank.QuestionTableData.Hindi.IsCorrectA;
                    question.OptionB = questionBank.QuestionTableData.Hindi.OptionB;
                    question.IsCorrectB = questionBank.QuestionTableData.Hindi.IsCorrectB;
                    question.OptionC = questionBank.QuestionTableData.Hindi.OptionC;
                    question.IsCorrectC = questionBank.QuestionTableData.Hindi.IsCorrectC;
                    question.OptionD = questionBank.QuestionTableData.Hindi.OptionD;
                    question.IsCorrectD = questionBank.QuestionTableData.Hindi.IsCorrectD;
                    question.Explanation = questionBank.QuestionTableData.Hindi.Explanation;
                    if (questionBank.QuestionType == QuestionType.SingleChoice)
                    {
                        question.IsPartiallyCorrect = false;
                        question.PartialThreeCorrectMark = 0;
                        question.PartialTwoCorrectMark = 0;
                        question.PartialOneCorrectMark = 0;
                    }
                    question.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                    question.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                    question.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                    question.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                    question.Mark = questionBank.Mark;
                    question.NegativeMark = questionBank.NegativeMark;
                    question.IsDeleted = false;
                    question.IsActive = true;
                    question.CreationDate = DateTime.UtcNow;
                    question.CreatorUserId = questionBank.UserId;
                    result = await _unitOfWork.Repository<DM.QuestionBank>().Insert(question);
                }
                if (questionBank.QuestionTableData.Gujarati != null)
                {
                    DM.QuestionBank question = new();
                    question.SubjectCategoryId = questionBank.SubjectCategoryId;
                    question.TopicId = questionBank.TopicId;
                    question.SubTopicId = questionBank.SubTopicId;
                    question.QuestionType = questionBank.QuestionType;
                    question.QuestionLevel = questionBank.QuestionLevel;
                    question.QuestionLanguage = QuestionLanguage.Gujarati.ToString();
                    question.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                    question.QuestionText = questionBank.QuestionTableData.Gujarati.QuestionText;
                    question.OptionA = questionBank.QuestionTableData.Gujarati.OptionA;
                    question.IsCorrectA = questionBank.QuestionTableData.Gujarati.IsCorrectA;
                    question.OptionB = questionBank.QuestionTableData.Gujarati.OptionB;
                    question.IsCorrectB = questionBank.QuestionTableData.Gujarati.IsCorrectB;
                    question.OptionC = questionBank.QuestionTableData.Gujarati.OptionC;
                    question.IsCorrectC = questionBank.QuestionTableData.Gujarati.IsCorrectC;
                    question.OptionD = questionBank.QuestionTableData.Gujarati.OptionD;
                    question.IsCorrectD = questionBank.QuestionTableData.Gujarati.IsCorrectD;
                    question.Explanation = questionBank.QuestionTableData.Gujarati.Explanation;
                    if (questionBank.QuestionType == QuestionType.SingleChoice)
                    {
                        question.IsPartiallyCorrect = false;
                        question.PartialThreeCorrectMark = 0;
                        question.PartialTwoCorrectMark = 0;
                        question.PartialOneCorrectMark = 0;
                    }
                    question.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                    question.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                    question.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                    question.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                    question.Mark = questionBank.Mark;
                    question.NegativeMark = questionBank.NegativeMark;
                    question.IsDeleted = false;
                    question.IsActive = true;
                    question.CreationDate = DateTime.UtcNow;
                    question.CreatorUserId = questionBank.UserId;
                    result = await _unitOfWork.Repository<DM.QuestionBank>().Insert(question);
                }
                if (questionBank.QuestionTableData.Marathi != null)
                {
                    DM.QuestionBank question = new();
                    question.SubjectCategoryId = questionBank.SubjectCategoryId;
                    question.TopicId = questionBank.TopicId;
                    question.SubTopicId = questionBank.SubTopicId;
                    question.QuestionType = questionBank.QuestionType;
                    question.QuestionLevel = questionBank.QuestionLevel;
                    question.QuestionLanguage = QuestionLanguage.Marathi.ToString();
                    question.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                    question.QuestionText = questionBank.QuestionTableData.Marathi.QuestionText;
                    question.OptionA = questionBank.QuestionTableData.Marathi.OptionA;
                    question.IsCorrectA = questionBank.QuestionTableData.Marathi.IsCorrectA;
                    question.OptionB = questionBank.QuestionTableData.Marathi.OptionB;
                    question.IsCorrectB = questionBank.QuestionTableData.Marathi.IsCorrectB;
                    question.OptionC = questionBank.QuestionTableData.Marathi.OptionC;
                    question.IsCorrectC = questionBank.QuestionTableData.Marathi.IsCorrectC;
                    question.OptionD = questionBank.QuestionTableData.Marathi.OptionD;
                    question.IsCorrectD = questionBank.QuestionTableData.Marathi.IsCorrectD;
                    question.Explanation = questionBank.QuestionTableData.Marathi.Explanation;
                    if (questionBank.QuestionType == QuestionType.SingleChoice)
                    {
                        question.IsPartiallyCorrect = false;
                        question.PartialThreeCorrectMark = 0;
                        question.PartialTwoCorrectMark = 0;
                        question.PartialOneCorrectMark = 0;
                    }
                    question.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                    question.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                    question.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                    question.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                    question.Mark = questionBank.Mark;
                    question.NegativeMark = questionBank.NegativeMark;
                    question.IsDeleted = false;
                    question.IsActive = true;
                    question.CreationDate = DateTime.UtcNow;
                    question.CreatorUserId = questionBank.UserId;
                    result = await _unitOfWork.Repository<DM.QuestionBank>().Insert(question);

                }
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// EditQuestionBank Method
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        public async Task<bool> EditQuestionBank(Req.EditQuestionBank questionBank)
        {
            if (questionBank != null)
            {
                int result = 0;
                if (questionBank.QuestionTableData.English != null)
                {
                    var question = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.Id == questionBank.QuestionTableData.English.Id && x.QuestionRefId == questionBank.QuestionTableData.QuestionRefId);
                    if (question != null)
                    {
                        question.SubjectCategoryId = questionBank.SubjectCategoryId;
                        question.TopicId = questionBank.TopicId;
                        question.SubTopicId = questionBank.SubTopicId;
                        question.QuestionLanguage = QuestionLanguage.English.ToString();
                        question.QuestionType = questionBank.QuestionType;
                        question.QuestionLevel = questionBank.QuestionLevel;
                        question.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                        question.QuestionText = questionBank.QuestionTableData.English.QuestionText;
                        question.OptionA = questionBank.QuestionTableData.English.OptionA;
                        question.IsCorrectA = questionBank.QuestionTableData.English.IsCorrectA;
                        question.OptionB = questionBank.QuestionTableData.English.OptionB;
                        question.IsCorrectB = questionBank.QuestionTableData.English.IsCorrectB;
                        question.OptionC = questionBank.QuestionTableData.English.OptionC;
                        question.IsCorrectC = questionBank.QuestionTableData.English.IsCorrectC;
                        question.OptionD = questionBank.QuestionTableData.English.OptionD;
                        question.IsCorrectD = questionBank.QuestionTableData.English.IsCorrectD;
                        question.Explanation = questionBank.QuestionTableData.English.Explanation;
                        if (questionBank.QuestionType == QuestionType.SingleChoice)
                        {
                            question.IsPartiallyCorrect = false;
                            question.PartialThreeCorrectMark = 0;
                            question.PartialTwoCorrectMark = 0;
                            question.PartialOneCorrectMark = 0;
                        }
                        question.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                        question.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                        question.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                        question.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                        question.Mark = questionBank.Mark;
                        question.NegativeMark = questionBank.NegativeMark;
                        question.IsDeleted = false;
                        question.IsActive = true;
                        question.LastModifyDate = DateTime.UtcNow;
                        question.LastModifierUserId = questionBank.UserId;
                        result = await _unitOfWork.Repository<DM.QuestionBank>().Update(question);
                    }
                }
                if (questionBank.QuestionTableData.Hindi != null)
                {
                    var hindiQuestion = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.Id == questionBank.QuestionTableData.Hindi.Id && x.QuestionRefId == questionBank.QuestionTableData.QuestionRefId && !x.IsDeleted);

                    if (hindiQuestion != null)
                    {
                        hindiQuestion.SubjectCategoryId = questionBank.SubjectCategoryId;
                        hindiQuestion.TopicId = questionBank.TopicId;
                        hindiQuestion.SubTopicId = questionBank.SubTopicId;
                        hindiQuestion.QuestionLanguage = QuestionLanguage.Hindi.ToString();
                        hindiQuestion.QuestionType = questionBank.QuestionType;
                        hindiQuestion.QuestionLevel = questionBank.QuestionLevel;
                        hindiQuestion.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                        hindiQuestion.QuestionText = questionBank.QuestionTableData.Hindi.QuestionText;
                        hindiQuestion.OptionA = questionBank.QuestionTableData.Hindi.OptionA;
                        hindiQuestion.IsCorrectA = questionBank.QuestionTableData.Hindi.IsCorrectA;
                        hindiQuestion.OptionB = questionBank.QuestionTableData.Hindi.OptionB;
                        hindiQuestion.IsCorrectB = questionBank.QuestionTableData.Hindi.IsCorrectB;
                        hindiQuestion.OptionC = questionBank.QuestionTableData.Hindi.OptionC;
                        hindiQuestion.IsCorrectC = questionBank.QuestionTableData.Hindi.IsCorrectC;
                        hindiQuestion.OptionD = questionBank.QuestionTableData.Hindi.OptionD;
                        hindiQuestion.IsCorrectD = questionBank.QuestionTableData.Hindi.IsCorrectD;
                        hindiQuestion.Explanation = questionBank.QuestionTableData.Hindi.Explanation;
                        if (questionBank.QuestionType == QuestionType.SingleChoice)
                        {
                            hindiQuestion.IsPartiallyCorrect = false;
                            hindiQuestion.PartialThreeCorrectMark = 0;
                            hindiQuestion.PartialTwoCorrectMark = 0;
                            hindiQuestion.PartialOneCorrectMark = 0;
                        }
                        hindiQuestion.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                        hindiQuestion.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                        hindiQuestion.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                        hindiQuestion.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                        hindiQuestion.Mark = questionBank.Mark;
                        hindiQuestion.NegativeMark = questionBank.NegativeMark;
                        hindiQuestion.IsDeleted = false;
                        hindiQuestion.IsActive = true;
                        hindiQuestion.LastModifyDate = DateTime.UtcNow;
                        hindiQuestion.LastModifierUserId = questionBank.UserId;
                        result = await _unitOfWork.Repository<DM.QuestionBank>().Update(hindiQuestion);
                    }
                }
                if (questionBank.QuestionTableData.Gujarati != null)
                {
                    var gujaratiQuestion = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.Id == questionBank.QuestionTableData.Gujarati.Id && x.QuestionRefId == questionBank.QuestionTableData.QuestionRefId && !x.IsDeleted);
                    if (gujaratiQuestion != null)
                    {


                        gujaratiQuestion.SubjectCategoryId = questionBank.SubjectCategoryId;
                        gujaratiQuestion.TopicId = questionBank.TopicId;
                        gujaratiQuestion.SubTopicId = questionBank.SubTopicId;
                        gujaratiQuestion.QuestionType = questionBank.QuestionType;
                        gujaratiQuestion.QuestionLevel = questionBank.QuestionLevel;
                        gujaratiQuestion.QuestionLanguage = QuestionLanguage.Gujarati.ToString();
                        gujaratiQuestion.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                        gujaratiQuestion.QuestionText = questionBank.QuestionTableData.Gujarati.QuestionText;
                        gujaratiQuestion.OptionA = questionBank.QuestionTableData.Gujarati.OptionA;
                        gujaratiQuestion.IsCorrectA = questionBank.QuestionTableData.Gujarati.IsCorrectA;
                        gujaratiQuestion.OptionB = questionBank.QuestionTableData.Gujarati.OptionB;
                        gujaratiQuestion.IsCorrectB = questionBank.QuestionTableData.Gujarati.IsCorrectB;
                        gujaratiQuestion.OptionC = questionBank.QuestionTableData.Gujarati.OptionC;
                        gujaratiQuestion.IsCorrectC = questionBank.QuestionTableData.Gujarati.IsCorrectC;
                        gujaratiQuestion.OptionD = questionBank.QuestionTableData.Gujarati.OptionD;
                        gujaratiQuestion.IsCorrectD = questionBank.QuestionTableData.Gujarati.IsCorrectD;
                        gujaratiQuestion.Explanation = questionBank.QuestionTableData.Gujarati.Explanation;
                        if (questionBank.QuestionType == QuestionType.SingleChoice)
                        {
                            gujaratiQuestion.IsPartiallyCorrect = false;
                            gujaratiQuestion.PartialThreeCorrectMark = 0;
                            gujaratiQuestion.PartialTwoCorrectMark = 0;
                            gujaratiQuestion.PartialOneCorrectMark = 0;
                        }
                        gujaratiQuestion.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                        gujaratiQuestion.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                        gujaratiQuestion.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                        gujaratiQuestion.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                        gujaratiQuestion.Mark = questionBank.Mark;
                        gujaratiQuestion.NegativeMark = questionBank.NegativeMark;
                        gujaratiQuestion.IsDeleted = false;
                        gujaratiQuestion.IsActive = true;
                        gujaratiQuestion.LastModifyDate = DateTime.UtcNow;
                        gujaratiQuestion.LastModifierUserId = questionBank.UserId;
                        result = await _unitOfWork.Repository<DM.QuestionBank>().Update(gujaratiQuestion);

                    }
                }
                if (questionBank.QuestionTableData.Marathi != null)
                {
                    var marathiQuestion = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.Id == questionBank.QuestionTableData.Marathi.Id && x.QuestionRefId == questionBank.QuestionTableData.QuestionRefId && !x.IsDeleted);
                    if (marathiQuestion != null)
                    {
                        marathiQuestion.SubjectCategoryId = questionBank.SubjectCategoryId;
                        marathiQuestion.TopicId = questionBank.TopicId;
                        marathiQuestion.SubTopicId = questionBank.SubTopicId;
                        marathiQuestion.QuestionType = questionBank.QuestionType;
                        marathiQuestion.QuestionLevel = questionBank.QuestionLevel;
                        marathiQuestion.QuestionLanguage = QuestionLanguage.Marathi.ToString();
                        marathiQuestion.QuestionRefId = questionBank.QuestionTableData.QuestionRefId;
                        marathiQuestion.QuestionText = questionBank.QuestionTableData.Marathi.QuestionText;
                        marathiQuestion.OptionA = questionBank.QuestionTableData.Marathi.OptionA;
                        marathiQuestion.IsCorrectA = questionBank.QuestionTableData.Marathi.IsCorrectA;
                        marathiQuestion.OptionB = questionBank.QuestionTableData.Marathi.OptionB;
                        marathiQuestion.IsCorrectB = questionBank.QuestionTableData.Marathi.IsCorrectB;
                        marathiQuestion.OptionC = questionBank.QuestionTableData.Marathi.OptionC;
                        marathiQuestion.IsCorrectC = questionBank.QuestionTableData.Marathi.IsCorrectC;
                        marathiQuestion.OptionD = questionBank.QuestionTableData.Marathi.OptionD;
                        marathiQuestion.IsCorrectD = questionBank.QuestionTableData.Marathi.IsCorrectD;
                        marathiQuestion.Explanation = questionBank.QuestionTableData.Marathi.Explanation;
                        if (questionBank.QuestionType == QuestionType.SingleChoice)
                        {
                            marathiQuestion.IsPartiallyCorrect = false;
                            marathiQuestion.PartialThreeCorrectMark = 0;
                            marathiQuestion.PartialTwoCorrectMark = 0;
                            marathiQuestion.PartialOneCorrectMark = 0;
                        }
                        marathiQuestion.IsPartiallyCorrect = questionBank.IsPartiallyCorrect;
                        marathiQuestion.PartialThreeCorrectMark = questionBank.PartialThreeCorrectMark;
                        marathiQuestion.PartialTwoCorrectMark = questionBank.PartialTwoCorrectMark;
                        marathiQuestion.PartialOneCorrectMark = questionBank.PartialOneCorrectMark;
                        marathiQuestion.Mark = questionBank.Mark;
                        marathiQuestion.NegativeMark = questionBank.NegativeMark;
                        marathiQuestion.IsDeleted = false;
                        marathiQuestion.IsActive = true;
                        marathiQuestion.LastModifyDate = DateTime.UtcNow;
                        marathiQuestion.LastModifierUserId = questionBank.UserId;
                        result = await _unitOfWork.Repository<DM.QuestionBank>().Update(marathiQuestion);
                    }
                }
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetQuestionBank by Id Method
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        public async Task<Res.QuestionBank?> GetByRefId(Req.GetQuestionBank questionBank)
        {
            Res.QuestionBank questinBanks = new();

            var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == questionBank.QuestionRefId && !x.IsDeleted);
            if (questions != null && questions.Count > 0)
            {

                var IdTypeList = await (from q in _unitOfWork.GetContext().SubjectCategories
                                        join s in _unitOfWork.GetContext().SubCourses on q.SubCourseId equals s.Id
                                        join course in _unitOfWork.GetContext().Courses on s.CourseID equals course.Id
                                        where q.Id == questions.First().SubjectCategoryId
                                        select new Res.TypesofId
                                        {
                                            SubCourseId = s.Id,
                                            CourseId = course.Id,
                                            ExamTypeId = course.ExamTypeID

                                        }).Distinct().FirstOrDefaultAsync();
                if (IdTypeList != null)
                {
                    questinBanks.ExamTypeId = IdTypeList.ExamTypeId;
                    questinBanks.CourseId = IdTypeList.CourseId;
                    questinBanks.SubCourseId = IdTypeList.SubCourseId;
                }
                questinBanks.SubjectCategoryId = questions.First().SubjectCategoryId;
                questinBanks.TopicId = questions.First().TopicId;
                questinBanks.SubTopicId = questions.First().SubTopicId;
                questinBanks.QuestionType = questions.First().QuestionType;
                questinBanks.QuestionLevel = questions.First().QuestionLevel.ToString();
                questinBanks.Mark = questions.First().Mark;
                questinBanks.NegativeMark = questions.First().NegativeMark;
                questinBanks.IsPartiallyCorrect = questions.First().IsPartiallyCorrect;
                questinBanks.PartialOneCorrectMark = questions.First().PartialOneCorrectMark;
                questinBanks.PartialTwoCorrectMark = questions.First().PartialTwoCorrectMark;
                questinBanks.PartialThreeCorrectMark = questions.First().PartialThreeCorrectMark;
                questinBanks.QuestionTableData.QuestionRefId = questions.First().QuestionRefId;
                foreach (var item in questions)
                {
                    switch (item.QuestionLanguage)
                    {
                        case "English":
                            Res.English english = new();
                            english.Id = item.Id;
                            english.QuestionText = item.QuestionText;
                            english.OptionA = item.OptionA;
                            english.OptionB = item.OptionB;
                            english.OptionC = item.OptionC;
                            english.OptionD = item.OptionD;
                            english.Explanation = item.Explanation;
                            english.IsCorrectA = item.IsCorrectA;
                            english.IsCorrectB = item.IsCorrectB;
                            english.IsCorrectC = item.IsCorrectC;
                            english.IsCorrectD = item.IsCorrectD;
                            questinBanks.QuestionTableData.English = english;
                            break;
                        case "Hindi":
                            Res.Hindi hindi = new();
                            hindi.Id = item.Id;
                            hindi.QuestionText = item.QuestionText;
                            hindi.OptionA = item.OptionA;
                            hindi.OptionB = item.OptionB;
                            hindi.OptionC = item.OptionC;
                            hindi.OptionD = item.OptionD;
                            hindi.IsCorrectA = item.IsCorrectA;
                            hindi.IsCorrectB = item.IsCorrectB;
                            hindi.IsCorrectC = item.IsCorrectC;
                            hindi.IsCorrectD = item.IsCorrectD;
                            hindi.Explanation = item.Explanation;
                            questinBanks.QuestionTableData.Hindi = hindi;
                            break;
                        case "Gujarati":
                            Res.Gujarati Gujarati = new();
                            Gujarati.Id = item.Id;
                            Gujarati.QuestionText = item.QuestionText;
                            Gujarati.OptionA = item.OptionA;
                            Gujarati.OptionB = item.OptionB;
                            Gujarati.OptionC = item.OptionC;
                            Gujarati.OptionD = item.OptionD;
                            Gujarati.IsCorrectA = item.IsCorrectA;
                            Gujarati.IsCorrectB = item.IsCorrectB;
                            Gujarati.IsCorrectC = item.IsCorrectC;
                            Gujarati.IsCorrectD = item.IsCorrectD;
                            Gujarati.Explanation = item.Explanation;
                            questinBanks.QuestionTableData.Gujarati = Gujarati;
                            break;
                        case "Marathi":
                            Res.Marathi marathi = new();
                            marathi.Id = item.Id;
                            marathi.QuestionText = item.QuestionText;
                            marathi.OptionA = item.OptionA;
                            marathi.OptionB = item.OptionB;
                            marathi.OptionC = item.OptionC;
                            marathi.OptionD = item.OptionD;
                            marathi.IsCorrectA = item.IsCorrectA;
                            marathi.IsCorrectB = item.IsCorrectB;
                            marathi.IsCorrectC = item.IsCorrectC;
                            marathi.IsCorrectD = item.IsCorrectD;
                            marathi.Explanation = item.Explanation;
                            questinBanks.QuestionTableData.Marathi = marathi;
                            break;
                        default:
                            break;
                    }
                }
                return questinBanks;
            }
            return null;

        }

        public async Task<Res.QuestionBankList?> GetAll(Req.GetAllQuestion questionBank)
        {
            Res.QuestionBankList questinBanks = new(); 

            var questionList = await (from q in _unitOfWork.GetContext().QuestionBanks
                                      join s in _unitOfWork.GetContext().SubjectCategories on q.SubjectCategoryId equals s.Id
                                      join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                      join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                      join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                      join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                      join subtopic in _unitOfWork.GetContext().SubTopics on topic.Id equals subtopic.TopicId
                                      where q.QuestionType == questionBank.QuestionType && q.QuestionLevel == questionBank.QuestionLevel
                                      && q.CreatorUserId == questionBank.CreatorUserId && !q.IsDeleted && s.Id == questionBank.SubjectCategoryId
                                      && topic.Id == questionBank.TopicId && subtopic.Id == questionBank.SubTopicId &&  q.SubjectCategoryId == questionBank.SubjectCategoryId   
                                      && q.TopicId == questionBank.TopicId && q.SubTopicId == questionBank.SubTopicId
                                      select new Res.AllQuestionBank
                                      {
                                          Id = q.QuestionRefId,
                                          QuestionType = q.QuestionType.ToString(),
                                          QuestionLevel = q.QuestionLevel.ToString(),
                                          CourseName = course.CourseName,
                                          SubCourseName = subCourse.SubCourseName,
                                          SubjectName = subject.SubjectName,
                                          CreatedBy = _userManager.FindByIdAsync(questionBank.CreatorUserId.ToString()).Result.DisplayName,
                                          //CreationDate=q.CreationDate,
                                          CreatorUserId=q.CreatorUserId
                                      }).Distinct().ToListAsync();

            if (questionList.Count > 0)
            {
                var result = questionList.OrderByDescending(x => x.Id);
                var resultV1 = result.Page(questionBank.PageNumber, questionBank.PageSize);
                questinBanks.QuestionBanks = resultV1.ToList();
                questinBanks.TotalRecords = questionList.Count;
                return questinBanks;
            }
            else
                return null;
        }
        /// <summary>
        /// GetAllQuestionBank Method
        /// </summary>
        /// <param name="questionBank"></param>
        /// <returns></returns>
        public Res.QuestionBankList? GetAll50(Req.GetAll50Question questionBank)
        {
            Res.QuestionBankList questinBanks = new();
            var questionList = (from q in _unitOfWork.GetContext().QuestionBanks.OrderByDescending(x => x.CreationDate)
                                join s in _unitOfWork.GetContext().SubjectCategories on q.SubjectCategoryId equals s.Id
                                join subject in _unitOfWork.GetContext().Subjects on s.SubjectId equals subject.Id
                                join subCourse in _unitOfWork.GetContext().SubCourses on s.SubCourseId equals subCourse.Id
                                join course in _unitOfWork.GetContext().Courses on subCourse.CourseID equals course.Id
                                join topic in _unitOfWork.GetContext().Topics on s.Id equals topic.SubjectCategoryId
                                join subtopic in _unitOfWork.GetContext().SubTopics on topic.Id equals subtopic.TopicId
                                where !q.IsDeleted && q.CreatorUserId != Guid.Empty
                                select new Res.AllQuestionBankV1
                                {
                                    Id = q.QuestionRefId,
                                    QuestionType = q.QuestionType.ToString(),
                                    QuestionLevel = q.QuestionLevel.ToString(),
                                    CourseName = course.CourseName,
                                    SubCourseName = subCourse.SubCourseName,
                                    SubjectName = subject.SubjectName,
                                    CreatedBy = q.CreatorUserId

                                }).AsQueryable().Take(300).DistinctBy(x => x.Id);

            List<Res.AllQuestionBank> quList = new();
            foreach (var it in questionList)
            {
                Res.AllQuestionBank qu = new()
                {
                    Id = it.Id,
                    QuestionType = it.QuestionType.ToString(),
                    QuestionLevel = it.QuestionLevel.ToString(),
                    CourseName = it.CourseName,
                    SubCourseName = it.SubCourseName,
                    SubjectName = it.SubjectName
                };
                var name = _userManager.FindByIdAsync(it.CreatedBy.ToString()).Result;
                qu.CreatedBy = name != null ? name.DisplayName : "N/A";
                quList.Add(qu);
            }
            if (quList.Count > 0)
            {
                var result = quList.Page(1, 50);
                var resultV1 = result.Page(questionBank.PageNumber, questionBank.PageSize);
                questinBanks.QuestionBanks = resultV1.ToList();
                questinBanks.TotalRecords = result.Count();
                return questinBanks;
            }
            else
            {
                return null;
            }
        }

        public string? GetName(string Id)
        {
            var name = _userManager.FindByIdAsync(Id).Result.DisplayName;
            return name;
        }

        /// <summary>
        /// DeleteQuestionByRefId Method
        /// </summary>
        /// <param name="bankRefId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteQuestionByRefId(Req.QuestionBankRefId bankRefId)
        {
            var questions = await _unitOfWork.Repository<DM.QuestionBank>().Get(x => x.QuestionRefId == bankRefId.QuestionRefId && !x.IsDeleted);
            int result = 0;
            if (questions != null)
            {
                foreach (var item in questions)
                {
                    item.IsActive = false;
                    item.IsDeleted = true;
                    item.DeleterUserId = bankRefId.UserId;
                    item.DeletionDate = DateTime.UtcNow;
                    result = await _unitOfWork.Repository<DM.QuestionBank>().Update(item);
                }
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// GetQuestionType Method
        /// </summary>
        /// <returns></returns>
        public List<EnumModel> GetQuestionType()
        {

            List<EnumModel> enList = new();
            List<EnumModel> enums = ((QuestionType[])Enum.GetValues(typeof(QuestionType))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            if (enums.Any())
            {
                foreach (var item in enums.Select(x => x.Value))
                {
                    EnumModel en = new()
                    {
                        Value = item,
                        Name = GetEnumName(item)
                    };
                    enList.Add(en);
                }
                return enList;
            }
            return enList;

        }

        public string GetEnumName(int value)
        {
            string name = "";
            switch (value)
            {
                case 1:
                    name = "Single Choice";
                    break;
                case 2:
                    name = "MCQ";
                    break;
                case 3:
                    name = "Integer Type";
                    break;
                case 4:
                    name = "Match The Column";
                    break;

                case 5:
                    name = "Phrases";
                    break;
            }
            return name;
        }

        /// <summary>
        /// GetQuestionLevel Method
        /// </summary>
        /// <returns></returns>
        public List<EnumModel> GetQuestionLevel()
        {
            List<EnumModel> enums = ((QuestionLevel[])Enum.GetValues(typeof(QuestionLevel))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }

        /// <summary>
        /// GetQuestionLanguage
        /// </summary>
        /// <returns></returns>
        public List<EnumModel> GetQuestionLanguage()
        {
            List<EnumModel> enums = ((QuestionLanguage[])Enum.GetValues(typeof(QuestionLanguage))).Select(c => new EnumModel() { Value = (int)c, Name = c.ToString() }).ToList();
            return enums;
        }

        /// <summary>
        /// CheckSubjectCategoryExist Method
        /// </summary>
        /// <param name="checkSubject"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubjectCategoryExist(CheckSubjectCategoryId checkSubject)
        {
            var SubjectCategory = await _unitOfWork.Repository<DM.SubjectCategory>().GetSingle(x => x.Id == checkSubject.Id && !x.IsDeleted);
            if (SubjectCategory != null)
                return true;
            return false;
        }

        /// <summary>
        /// CheckTopicExist Method
        /// </summary>
        /// <param name="checkTopic"></param>
        /// <returns></returns>
        public async Task<bool> CheckTopicExist(CheckTopicId checkTopic)
        {
            var isTopicExist = await _unitOfWork.Repository<DM.Topic>().GetSingle(x => x.Id == checkTopic.Id && !x.IsDeleted);
            if (isTopicExist != null)
                return true;
            return false;
        }

        /// <summary>
        /// checkSubtopic Method
        /// </summary>
        /// <param name="checkSubtopic"></param>
        /// <returns></returns>
        public async Task<bool> CheckSubTopicExists(CheckSubtopicId checkSubtopic)
        {
            var isSubTopicExist = await _unitOfWork.Repository<DM.SubTopic>().GetSingle(x => x.Id == checkSubtopic.Id && !x.IsDeleted);
            if (isSubTopicExist != null)
                return true;
            return false;
        }

        /// <summary>
        /// CheckRefernceIdExists Method
        /// </summary>
        /// <param name="checkReference"></param>
        /// <returns></returns>
        public async Task<bool> CheckReferenceIdExists(CheckReference checkReference)
        {
            var isRefIdExists = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == checkReference.ReferenceId && !x.IsDeleted);
            if (isRefIdExists != null)
                return true;
            return false;

        }

        /// <summary>
        /// CheckRefIdExists Method
        /// </summary>
        /// <param name="checkReference"></param>
        /// <returns></returns>
        public async Task<bool> CheckRefIdExists(CheckReference checkReference)
        {
            var isRefIdExist = await _unitOfWork.Repository<DM.QuestionBank>().GetSingle(x => x.QuestionRefId == checkReference.ReferenceId && !x.IsDeleted);
            if (isRefIdExist != null)
                return true;
            return false;

        }

        /// <summary>
        /// CheckUserIdExists Method
        /// </summary>
        /// <param name="userExist"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserIdExists(CheckUserExist userExist)
        {
            var isUserIdExists = await _userManager.FindByIdAsync(userExist.Id.ToString());
            if (isUserIdExists != null && !isUserIdExists.IsDeleted)
                return true;
            return false;

        }


    }
}
