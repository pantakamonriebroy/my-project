using ChillPay.Merchant.Register.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories
{
    internal class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        private DbSet<TEntity> _set;
        private bool _disposed;

        internal Repository(ChillPayGlobalDbContext context)
        {
            Context = context;
            _set = Context.Set<TEntity>();
        }

        protected ChillPayGlobalDbContext Context { get; }

        protected DbSet<TEntity> Set => _set ?? (_set = Context.Set<TEntity>());

        public async Task AddAsync(TEntity entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Set.Add(entity);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Set.Attach(entity);
                entry = Context.Entry(entity);
            }

            entry.State = EntityState.Modified;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Set.Remove(entity);

            await Context.SaveChangesAsync();
        }

        public virtual async Task<long> CountAllAsync()
        {
            return await Set.LongCountAsync();
        }

        public virtual async Task<TEntity> FindByIdAsync(TKey id)
        {
            return await Set.FindAsync(id);
        }
        public virtual async Task<TEntity> FindByUserIdAsync(TKey userid)
        {
            return await Set.FindAsync(userid);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Set.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> PageAllAsync(int page, int pageSize)
        {
            page = (page > 0) ? page - 1 : 0;
            return await Set.Skip(page * pageSize).Take(pageSize).ToListAsync();
        }
        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }

                _disposed = true;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        #endregion
    }


}
