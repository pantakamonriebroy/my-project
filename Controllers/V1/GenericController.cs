using ChillPay.Merchant.Register.Api.Configs;
using ChillPay.Merchant.Register.Api.Domains;
using ChillPay.Merchant.Register.Api.Entities.Generics;
using ChillPay.Merchant.Register.Api.Models;
using ChillPay.Merchant.Register.Api.Models.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChillPay.Merchant.Register.Api.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController : BaseController
    {
        private readonly ILogger<GenericController> _logger;
        private readonly IUserService _userService;

        public GenericController(IUserService userService, IOptions<AppSettings> settings, ILogger<GenericController> logger)
            : base(settings, logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("province/{language=TH}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<JsonProvinceModel[]>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetProvince(string language)
        {
            _logger.LogInformation($"[GetProvince] language={language}");
            var jsonResponse = ApiResponseMessageModel<JsonProvinceModel[]>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // validate parameter
                jsonError = ValidateLanguage(language);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                language = language.ToUpper();

                List<Province> pList = await _userService.GenericRepository.GetAllProvinceAsync();

                // create result list
                List<JsonProvinceModel> resList = new List<JsonProvinceModel>();
                foreach (Province p in pList)
                {
                    resList.Add(new JsonProvinceModel
                    {
                        Id = p.Id,
                        Name = (string.Compare(language, "TH") == 0) ? p.NameTh : p.NameEn,
                    });
                }

                jsonResponse = ApiResponseMessageModel<JsonProvinceModel[]>.Success(resList.ToArray());
                jsonResponse.TotalRecord = resList.Count();

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("district/{provinceId:int}/{language=TH}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<JsonDistrictModel[]>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetDistrict(int provinceId, string language)
        {
            _logger.LogInformation($"GetDistrict: provinceId={provinceId}, language={language}");
            var jsonResponse = ApiResponseMessageModel<JsonDistrictModel[]>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // validate parameter
                jsonError = ValidateLanguage(language);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                language = language.ToUpper();

                List<District> pList = await _userService.GenericRepository.GetAllDistrictByProvinceIdAsync(provinceId);

                // create result list
                List<JsonDistrictModel> resList = new List<JsonDistrictModel>();
                foreach (District p in pList)
                {
                    resList.Add(new JsonDistrictModel
                    {
                        Id = p.Id,
                        Name = (string.Compare(language, "TH") == 0) ? p.NameInThai : p.NameInEnglish,
                        ProvinceId = provinceId,
                    });
                }

                jsonResponse = ApiResponseMessageModel<JsonDistrictModel[]>.Success(resList.ToArray());
                jsonResponse.TotalRecord = resList.Count();

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("subdistrict/{districtId:int}/{language=TH}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<JsonSubdistrictModel[]>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetSubdistrictVoil(int districtId, string language)
        {
            _logger.LogInformation($"GetSubdistrict: districtId={districtId}, language={language}");
            var jsonResponse = ApiResponseMessageModel<JsonSubdistrictModel[]>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // validate parameter
                jsonError = ValidateLanguage(language);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                language = language.ToUpper();

                List<Subdistrict> pList = await _userService.GenericRepository.GetAllSubdistrictByDistrictIdAsync(districtId);

                // create result list
                List<JsonSubdistrictModel> resList = new List<JsonSubdistrictModel>();
                foreach (Subdistrict p in pList)
                {
                    resList.Add(new JsonSubdistrictModel
                    {
                        Id = p.Id,
                        Name = (string.Compare(language, "TH") == 0) ? p.NameInThai : p.NameInEnglish,
                        ZipCode = p.ZipCode,
                        DistrictId = districtId,
                    });
                }

                jsonResponse = ApiResponseMessageModel<JsonSubdistrictModel[]>.Success(resList.ToArray());
                jsonResponse.TotalRecord = resList.Count();

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }


        private ApiResponseMessageModel<string> ValidateLanguage(string language)
        {
            var jsonResponse = ApiResponseMessageModel<string>.Failed();
            jsonResponse.Message = "Invalid parameter";

            if (language == null)
            {
                _logger.LogError("ERROR: language is null.");

                return jsonResponse;
            }

            string[] langSupport = { "EN", "TH" };
            if (!langSupport.Contains(language.ToUpper()))
            {
                _logger.LogError("ERROR: language is incorrect format.");
                jsonResponse.Message = "Invalid language.";

                return jsonResponse;
            }

            return ApiResponseMessageModel.Success("Success");
        }
    }
}
