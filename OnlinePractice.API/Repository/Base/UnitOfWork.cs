using Microsoft.EntityFrameworkCore;
using OnlinePractice.API.Models;


namespace OnlinePractice.API.Repository.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _Context;
        public UnitOfWork(DBContext context)
        {
            _Context = context;

        }


        private readonly Dictionary<Type, object> _Repositories = new();
        public GenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            // Check to see if we have a constructor for the given type
            if (!_Repositories.ContainsKey(typeof(TEntity)))
            {
                _Repositories.Add(typeof(TEntity), new GenericRepository<TEntity>(_Context));
            }
            return (GenericRepository<TEntity>)_Repositories[typeof(TEntity)];
        }

        public async Task Save()
        {
            await _Context.SaveChangesAsync();
        }

        public void Detach(object obj)
        {
            _Context.Entry(obj).State = EntityState.Detached;
        }

        public DBContext GetContext()
        {
            return _Context;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Context.Dispose();
            }
        }
    }
}
