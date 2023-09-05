using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePractice.API.Models.DBModel
{
    public class StudentCourse : BaseModel
    {
        public Guid StudentId { get; set; }
        public Guid SubCourseId { get; set; }
        public Guid InstituteId { get; set; }
    }
}
