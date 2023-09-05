using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlinePractice.API.Models;
using OnlinePractice.API.Models.AuthDB;
using OnlinePractice.API.Models.DBModel;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Base
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }



        #region Tables
        public DbSet<Product> Products => Set<Product>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<StudyMaterial> StudyMaterials => Set<StudyMaterial>();
        public DbSet<Ebook> Ebooks => Set<Ebook>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<SubCourse> SubCourse => Set<SubCourse>();
        public DbSet<ExamType> ExamTypes => Set<ExamType>();
        public DbSet<Institute> Institutes => Set<Institute>();
        public DbSet<Countries> Countries => Set<Countries>();
        public DbSet<State> States => Set<State>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<SubCourse> SubCourses => Set<SubCourse>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<SubTopic> SubTopics => Set<SubTopic>();
        public DbSet<QuestionBank> QuestionBanks => Set<QuestionBank>();
        public DbSet<SubjectCategory> SubjectCategories => Set<SubjectCategory>();
        public DbSet<ExamPattern>ExamPattern => Set<ExamPattern>();
        public DbSet<ExamPatternSection> ExamPatternSections => Set<ExamPatternSection>();
        public DbSet<MockTestSettings> MockTestSettings => Set<MockTestSettings>();
        public DbSet<MockTestQuestions> MockTestQuestions => Set<MockTestQuestions>();
        public DbSet<PreviousYearPaper> PreviousYearPapers => Set<PreviousYearPaper>();
        public DbSet<StudentCourse> StudentCourse => Set<StudentCourse>();
        public DbSet<Token> Tokens=> Set<Token>();

        public DbSet<StudentMockTest> StudentMockTests => Set<StudentMockTest>();

        public DbSet<StudentMockTestQuestions> StudentMockTestQuestions=> Set<StudentMockTestQuestions>();

        public DbSet<StudentAnswers> StudentAnswers=> Set<StudentAnswers>();

        public DbSet<StudentFeedbacks> StudentFeedbacks=> Set<StudentFeedbacks>();

        public DbSet<StudentMockTestStatus> StudentMockTestStatuses=> Set<StudentMockTestStatus>();

        public DbSet<StudentResult> StudentResults=> Set<StudentResult>();
        public DbSet<MyCart> MyCart => Set<MyCart>();
        public DbSet<MyPurchased> MyPurchaseds => Set<MyPurchased>();
        public DbSet<StudentRank> StudentRanks => Set<StudentRank>();
        public DbSet<WalletHistory> WalletHistory => Set<WalletHistory>();
        public DbSet<StudentMocktestRank> StudentMocktestRanks=> Set<StudentMocktestRank>();
        public DbSet<Payment> Payments=> Set<Payment>();

        #endregion

        #region

      //  public DbSet<Res.StudentEbook> studentEbooks=> Set<Res.StudentEbook>();

        #endregion
        public virtual async Task<int> SaveChangesAsync()
        {
            OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Modified)
                    continue;
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name
                };
                var changeEntity = (BaseModel)entry.Entity;
                auditEntry.UserId = changeEntity.LastModifierUserId;
                auditEntries.Add(auditEntry);
                foreach (PropertyEntry property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey() && property.CurrentValue != null)
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    if (property.IsModified && property.CurrentValue != null && property.OriginalValue != null)
                    {
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}
