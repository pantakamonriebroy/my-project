using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterAddressRepository : Repository<RegisterAddress, long>, IRegisterAddressRepository
    {
        private readonly ILogger _logger;

        internal RegisterAddressRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterAddress address)
        {
            try
            {
                if (address.Id > 0)
                {
                    await UpdateAsync(address);
                }
                else
                {
                    await AddAsync(address);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        //public async Task<List<RegisterAddress>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        //{
        //    var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.Id);
        //    return await query.ToListAsync();
        //}

        public async Task<RegisterAddress> GetLatestByAddressTypeAsync(long registerMerchantId, byte addressType)
        {
            //Expression<Func<RegisterAddress, object>> expr = p => p.ModifiedDate.HasValue ? p.ModifiedDate.Value : p.AddedDate;
            //var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.AddressType == addressType).OrderByDescending(expr);
            var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.AddressType == addressType);
            return await query.FirstOrDefaultAsync();
        }

        //public async Task<List<RegisterAddress>> GetAllByAddressTypeAsync(long registerMerchantId, byte addressType)
        //{
        //    var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.AddressType == addressType).OrderBy(m => m.Id);
        //    return await query.ToListAsync();
        //}

        //public async Task<RegisterAddress2Model> GetLatestByAddressTypeAsync2(long registerMerchantId, byte addressType)
        //{
        //    Expression<Func<RegisterAddress, object>> expr = p => p.ModifiedDate.HasValue ? p.ModifiedDate.Value : p.AddedDate;
        //    var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.AddressType == addressType).OrderByDescending(expr);

        //    var query2 = query.Join(Context.Provinces,
        //        ra => ra.ProvinceId,
        //        pr => pr.Id,
        //        (ra, province) => new
        //        {
        //            Address = ra,
        //            Province = province,
        //        });
        //    var query3 = query2.Join(Context.Districts,
        //        ra => ra.Address.DistrictId,
        //        dt => dt.Id,
        //        (ra, district) => new
        //        {
        //            ra.Address,
        //            ra.Province,
        //            District = district,
        //        });
        //    var query4 = query3.Join(Context.Subdistricts,
        //        ra => ra.Address.SubdistrictId,
        //        sd => sd.Id,
        //        (ra, subdistrict) => new RegisterAddress2Model
        //        {
        //            Address = ra.Address.Address,
        //            ProvinceId = ra.Address.ProvinceId,
        //            DistrictId = ra.Address.DistrictId,
        //            SubdistrictId = ra.Address.SubdistrictId,
        //            TelNo = ra.Address.TelNo,
        //            Fax = ra.Address.Fax,
        //            Province = (ra.Province != null) ? ra.Province.NameTh : "",
        //            District = (ra.District != null) ? ra.District.NameInThai : "",
        //            Subdistrict = (subdistrict != null) ? subdistrict.NameInThai : "",
        //            ZipCode = (subdistrict != null && subdistrict.ZipCode.HasValue) ? subdistrict.ZipCode.Value.ToString() : "",
        //        });

        //    return await query4.FirstOrDefaultAsync();
    }
}

