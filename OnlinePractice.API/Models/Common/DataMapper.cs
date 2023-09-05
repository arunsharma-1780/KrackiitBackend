using AutoMapper;
using DM = OnlinePractice.API.Models.DBModel;
using Req = OnlinePractice.API.Models.Request;
using Res = OnlinePractice.API.Models.Response;

namespace OnlinePractice.API.Models.Common
{
    public class DataMapper
    {

        public static IMapper Mapper { get; private set; }
        static DataMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {

                #region DataModel To ViewModel
                cfg.CreateMap<DM.QuestionBank, Res.QuestionBank>();


                #endregion


                #region ViewModel to DataModel
                cfg.CreateMap<Res.QuestionBank, DM.QuestionBank>();

                #endregion
            });
            Mapper = config.CreateMapper();
        }
    }
}
