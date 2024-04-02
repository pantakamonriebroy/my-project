using ChillPay.Merchant.Register.Api.Domains.Users;
using ChillPay.Merchant.Register.Api.Entities.Customers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Validate
{
    internal class CustomerValidator /*: IIdentityValidator<Customer>*/
    {
        //private ICustomerRepository _manager;

        //public CustomerValidator(ICustomerRepository manager)
        //{
        //    _manager = manager ?? throw new ArgumentNullException("manager");
        //}

        //public async Task<IdentityResult> ValidateAsync(Customer customer)
        //{
        //    var errors = new List<IdentityError>();
        //    await ValidateEmailAsync(customer, errors);

        //    if (errors.Count > 0)
        //    {
        //        return IdentityResult.Failed(errors.ToArray());
        //    }

        //    var cust = await _manager.FindByEmailAsync(customer.Email);
        //    if (cust != null && cust.Id != customer.Id)
        //    {
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = string.Format("Cannot insert duplicate key 'Email' in 'Customers'. The duplicate key value is ({0}).", customer.Email) });
        //    }

        //    return IdentityResult.Success;
        //}

        //private Task ValidateEmailAsync(Customer customer, List<IdentityError> errors)
        //{
        //    if (customer == null || string.IsNullOrWhiteSpace(customer.Email))
        //    {
        //        errors.Add(new IdentityError { Code = "500", Description = string.Format(CultureInfo.CurrentCulture, "{0} cannot be null or empty.", "Email") });
        //        return Task.FromResult(0);
        //    }

        //    try
        //    {
        //        var m = new MailAddress(customer.Email);
        //    }
        //    catch (FormatException)
        //    {
        //        errors.Add(new IdentityError { Code = "500", Description = string.Format(CultureInfo.CurrentCulture, "Email '{0}' is invalid.", customer.Email) });
        //        return Task.FromResult(0);
        //    }

        //    return Task.FromResult(0);
        //}
    }
}
