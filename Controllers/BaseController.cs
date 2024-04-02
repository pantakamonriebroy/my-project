using ChillPay.Merchant.Register.Api.Configs;
using ChillPay.Merchant.Register.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ChillPay.Merchant.Register.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;

        protected const string CHILLPAY_HEADER_KEY = "CHILLPAY-TOKEN";

        protected BaseController(IOptions<AppSettings> settings, ILogger logger)
        {
            _logger = logger;
            _appSettings = settings.Value;
        }

        protected string GetHeaderValue(string key)
        {
            if (Request.Headers.TryGetValue(key, out StringValues value))
            {
                return value.FirstOrDefault();
            }

            return string.Empty;
        }

        protected ApiResponseMessageModel<string> ValidateHeader()
        {
            var jsonResponse = ApiResponseMessageModel<string>.Failed();
            var requestToken = GetHeaderValue(CHILLPAY_HEADER_KEY);

            if (string.IsNullOrEmpty(requestToken))
            {
                jsonResponse.Message = "Parameter token has empty";
                _logger.LogError("ERROR: Parameter token has empty.");

                return jsonResponse;
            }

            if (!requestToken.Equals(_appSettings.CHILLPAY_TOKEN))
            {
                jsonResponse.Message = "Invalid token key";
                _logger.LogError("ERROR: Invalid token key. {0}", requestToken);

                return jsonResponse;
            }

            return ApiResponseMessageModel.Success("Success");
        }
    }
}
