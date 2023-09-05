using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;
using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Repository.Base;
using OnlinePractice.API.Models.Request;
using OnlinePractice.API.Models.DBModel;
using OnlinePractice.API.Repository.Interfaces;
using OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Repository.Services
{

    public class StudyMaterialRepository : IStudyMaterialRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudyMaterialRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// create product data
        /// </summary>
        /// <param name="createProduct"></param>
        /// <returns></returns>
        public async Task<bool> Create(Req.CreateStudyMaterial studyMaterial)
        {
            var course = await _unitOfWork.Repository<DM.Course>().GetByID(studyMaterial.CourseId);
            if (course == null) return false;
            DM.StudyMaterial material = new()
            {
                Title=studyMaterial.Title,
                Description=studyMaterial.Description,
                FormatType=studyMaterial.FormatType,
                Price=studyMaterial.Price,
                Course = course,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CreatorUserId = studyMaterial.UserId,
            };
            int result = await _unitOfWork.Repository<DM.StudyMaterial>().Insert(material);
            return result > 0;
        }


        /// <summary>
        /// Edit Study Material
        /// </summary>
        /// <param name="editStudyMaterial"></param>
        /// <returns></returns>
        public async Task<bool> Edit(Req.EditStudyMaterial studyMaterial)
        {

            var material = await _unitOfWork.Repository<DM.StudyMaterial>().GetSingle(x => x.Id == studyMaterial.Id && !x.IsDeleted);
            var course = await _unitOfWork.Repository<DM.Course>().GetSingle(x => x.Id == studyMaterial.CourseId && !x.IsDeleted);
            if (material != null && course != null)
            {
                material.Title = studyMaterial.Title;
                material.Description = studyMaterial.Description;
                material.FormatType = studyMaterial.FormatType;
                material.Price = studyMaterial.Price;
                material.Course = course;
                material.LastModifierUserId = studyMaterial.UserId;
                material.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.StudyMaterial>().Update(material);
                return result > 0;
            }
            return false;
        }


        // <summary>
        // Get requested product data by id
        // </summary>
        // <param name="id"></param>
        // <returns></returns>
        public async Task<Models.Response.StudyMaterial?> GetById(MaterialById material)
        {
            var materialData = await _unitOfWork.Repository<DM.StudyMaterial>().GetSingle(x => x.Id == material.Id && !x.IsDeleted);
            if (materialData != null)
            {
                Models.Response.StudyMaterial result = new()
                {
                    Id = materialData.Id,
                    Title = materialData.Title,
                    Description = materialData.Description,
                    FormatType=materialData.FormatType,
                    Price = materialData.Price,
                }; 
                return result;
            }
            return null;
        }


        /// <summary>
        /// Deleted requested product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(MaterialById material)
        {
            var materialData = await _unitOfWork.Repository<DM.StudyMaterial>().GetSingle(x => x.Id == material.Id && !x.IsDeleted);
            if (materialData != null)
            {
                materialData.IsDeleted = true;
                materialData.DeleterUserId = material.UserId;
                materialData.DeletionDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.StudyMaterial>().Update(materialData);
                return result > 0;
            }
            return false;
        }


        /// <summary>
        /// Edit Study Material Price
        /// </summary>
        /// <param name="editPriceStudy"></param>
        /// <returns></returns>
        public async Task<bool> EditPrice(EditPriceStudyMaterial studyMaterial)
        {
            var materialData=await _unitOfWork.Repository<DM.StudyMaterial>().GetSingle(x => x.Id == studyMaterial.Id && !x.IsDeleted);
            if (materialData != null)
            {
                materialData.Price= studyMaterial.Price;
                materialData.LastModifierUserId = studyMaterial.UserId;
                materialData.LastModifyDate = DateTime.UtcNow;
                int result = await _unitOfWork.Repository<DM.StudyMaterial>().Update(materialData);
                return result > 0;
            }
            return false;
        }

    }

}
