namespace ChillPay.Merchant.Register.Api.Domains.Repositories
{
    public interface IRepository<TEntity, TKey> : IDisposable where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> PageAllAsync(int page, int pageSize);
        Task<TEntity> FindByIdAsync(TKey id);
        Task<TEntity> FindByUserIdAsync(TKey userid);

        //long CountAll();
        Task<long> CountAllAsync();

        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
