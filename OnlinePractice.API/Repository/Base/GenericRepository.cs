using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OnlinePractice.API.Repository.Base
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal DBContext context;
        internal DbSet<TEntity> dbSet;
        internal Expression<Func<TEntity, bool>>? globalFilter;
        internal Func<TEntity, bool>? MatchesFilter { get; private set; }

        public GenericRepository(DBContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public GenericRepository(DBContext context, Expression<Func<TEntity, bool>> filter)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
            globalFilter = filter;
            MatchesFilter = filter.Compile();
        }

        #region -- Public Methods --

        #region Get Method
        /// <summary>
        /// Retrive all records with filter and including relation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includeGlobalFilter">Set to true to include Deleted items (otherwise global filter excludes them)</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "", int? skip = null, int? take = null, bool includeGlobalFilter = true)
        {
            IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, includeGlobalFilter);

            if (skip.HasValue && skip.Value > 0) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Create dynamic query with filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includeGlobalFilter">Set to true to include Deleted items (otherwise global filter excludes them)</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "", int? skip = null, int? take = null, bool includeGlobalFilter = true)
        {
            IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, includeGlobalFilter);

            if (skip.HasValue && skip.Value > 0) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return query;
        }

        /// <summary>
        /// Get single record by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="includeGlobalFilter">Set to true to include Deleted items (otherwise global filter excludes them)</param>
        /// <returns></returns>
        public virtual async Task<TEntity?> GetSingle(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "", bool includeGlobalFilter = true)
        {
            IQueryable<TEntity> query = BuildQuery(filter, orderBy, includeProperties, includeGlobalFilter);
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get count from requested entity with filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includeGlobalFilter"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCount(Expression<Func<TEntity, bool>>? filter = null, bool includeGlobalFilter = true)
        {
            IQueryable<TEntity> query = BuildQuery(filter, null, "", includeGlobalFilter);
            return await query.CountAsync();
        }

        /// <summary>
        /// Get record from requested entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity?> GetByID(object id)
        {
            return await dbSet.FindAsync(id);
        }
        #endregion

        #region Insert Method
        /// <summary>
        /// Insert single record
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> Insert(TEntity entity)
        {
            dbSet.Add(entity);
            var result = await context.SaveChangesAsync();

            return result;
        }
        /// <summary>
        /// Insert multiple records
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> Insert(List<TEntity> entity)
        {
            dbSet.AddRange(entity);
            var result = await context.SaveChangesAsync();

            return result;
        }
        #endregion

        #region Delete Method
        /// <summary>
        /// Delete by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<int> Delete(object id)
        {
            TEntity? entityToDelete = await dbSet.FindAsync(id);
            var result = await Delete(entityToDelete);

            return result;
        }
        /// <summary>
        /// Delete by entity
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public virtual async Task<int> Delete(TEntity? entityToDelete)
        {
            if (entityToDelete == null)
                return 0;
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);

            return await context.SaveChangesAsync();
        }
        /// <summary>
        /// Delete multiple entity
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task Delete(List<TEntity> list)
        {
            dbSet.RemoveRange(list);
            await context.SaveChangesAsync();
        }

        #endregion

        #region Update Method
        /// <summary>
        /// update single entity
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual async Task<int> Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;

            return await context.SaveChangesAsync();
        }
        /// <summary>
        /// Update multiple entity with relation
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual async Task<int> Update(List<TEntity> entityToUpdate)
        {
            foreach (var item in entityToUpdate)
            {
                dbSet.Attach(item);
                context.Entry(item).State = EntityState.Modified;
            }

            return await context.SaveChangesAsync();
        }
        #endregion

        #endregion

        #region -- Private Methods --
        /// <summary>
        /// Supporting method for build query
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="includeGlobalFilter">Set to true to include Deleted items (otherwise global filter excludes them)</param>
        /// <returns></returns>
        private IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "", bool includeGlobalFilter = true)
        {
            IQueryable<TEntity> query = dbSet;

            if (globalFilter != null && includeGlobalFilter)
            {
                query = query.Where(globalFilter);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        #endregion                               
    }
}
