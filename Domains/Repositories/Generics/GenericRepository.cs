using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Generics;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Generics
{
    internal class GenericRepository : IGenericRepository
    {
        private readonly ILogger _logger;
        private readonly ChillPayGlobalDbContext Context;
        private bool _disposed;

        public GenericRepository(ChillPayGlobalDbContext context, ILogger logger)
        {
            Context = context;
            _logger = logger;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(_disposed);
        }

        //public async Task<IdentityResult> CreateOrUpdateContactAsync(Contact contact)
        //{
        //    try
        //    {
        //        ThrowIfDisposed();
        //        if (contact == null)
        //        {
        //            throw new ArgumentNullException("entity");
        //        }

        //        if (contact.Id > 0)
        //        {
        //            var entry = Context.Entry(contact);
        //            if (entry.State == EntityState.Detached)
        //            {
        //                Context.Contacts.Attach(contact);
        //                entry = Context.Entry(contact);
        //            }
        //            entry.State = EntityState.Modified;
        //        }
        //        else
        //        {
        //            Context.Contacts.Add(contact);
        //        }

        //        await Context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //    }

        //    return IdentityResult.Success;
        //}

        public List<Province> GetAllProvince()
        {
            return Context.Provinces.ToList();
        }

        public async Task<List<Province>> GetAllProvinceAsync()
        {
            return await Context.Provinces.ToListAsync();
        }

        public async Task<List<District>> GetAllDistrictByProvinceIdAsync(int provinceId)
        {
            var query = Context.Districts.Where(m => m.ProvinceId == provinceId);
            return await query.ToListAsync();
        }

        public async Task<List<Subdistrict>> GetAllSubdistrictByDistrictIdAsync(int districtId)
        {
            var query = Context.Subdistricts.Where(m => m.DistrictId == districtId);
            return await query.ToListAsync();
        }
        //add
        public async Task<List<Subdistrict>> GetAllSubdistrictByID(int SubdistrictID)
        {
            var query = Context.Subdistricts.Where(m => m.Id == SubdistrictID);
            return await query.ToListAsync();
        }

    }
}
