using OnlinePractice.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePractice.API.Repository.Base
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task Save();
        void Detach(object obj);
        DBContext GetContext();
    }
}
