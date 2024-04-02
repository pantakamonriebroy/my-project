using ChillPay.Merchant.Register.Api.Domains.Repositories.Registers;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using ChillPay.Merchant.Register.Api.Validate;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Validator
{
    internal class RegisterMerchantValidator : IIdentityValidator<RegisterMerchant>
    {
        private IRegisterMerchantRepository _manager;

        public RegisterMerchantValidator(IRegisterMerchantRepository manager)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
        }

        public async Task<IdentityResult> ValidateAsync(RegisterMerchant item)
        {
            var regM = await _manager.FindByUserIdAsync(item.UserId);
            if (regM != null && regM.Id != item.Id)
            {
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = string.Format("Cannot insert duplicate key 'UserId' in 'RegisterMerchant'. The duplicate key value is ({0}).", item.UserId) });
            }

            return IdentityResult.Success;
        }
    }
}
