using BloodBank.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BloodDoner.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #region Login
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginListDTO loginListDTO)
        {
            var validationErrors = new List<string>();

            if (!ModelState.IsValid)
            {
                 validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewData["ValidationErrors"] = validationErrors;
                return View(loginListDTO);
            }

            Response<ResponseLoginListDTO> objResponse = null;

            try
            {
                var jsonPayload = JsonSerializer.Serialize(loginListDTO);
                var client = _httpClientFactory.CreateClient();
                var apiUrl = "https://localhost:44398/api/DonerLogin";
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    objResponse = JsonSerializer.Deserialize<Response<ResponseLoginListDTO>>(responseData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (objResponse?.Data != null)
                    {
                        var jwtToken = objResponse.Data.JWTToken;

                        if (jwtToken != null)
                        {
                            HttpContext.Session.SetString("JWTToken", jwtToken);

                            return RedirectToAction("AvailableDoner", "Donor");
                        }
                        else
                        {
                            ModelState.AddModelError("", objResponse.Status);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", objResponse.Status);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error during login process. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred during the login process.");
                return View("Error");
            }

            // Return the view with the model and validation errors

            validationErrors = ModelState.Values
       .SelectMany(v => v.Errors)
       .Select(e => e.ErrorMessage)
       .ToList();
            ViewData["ValidationErrors"] = validationErrors;

            // Return the view with the model and validation errors
            return View(loginListDTO);
        }
        #endregion

        #region RegisterDoner
        [HttpGet("RegisterDoner")]
        public async Task<IActionResult> RegisterDoner()
        {
            await PopulateViewDataAsync();
            return View();
        }

        #region Private Methods
        private async Task PopulateViewDataAsync()
        {
            var stateApiUrl = "https://localhost:44398/api/GetState";
            var lstStatelistDTO = await FetchDataFromApi<StatelistDTO>(stateApiUrl);

            var bloodGroupApiUrl = "https://localhost:44398/api/GetBloodGroup";
            var lstGetBloodGroupListDTO = await FetchDataFromApi<GetBloodGroupListDTO>(bloodGroupApiUrl);

            ViewData["States"] = new SelectList(lstStatelistDTO, "StateId", "State");
            ViewData["BloodGroups"] = new SelectList(lstGetBloodGroupListDTO, "BloodGroupId", "BloodGroup");
        }
        #endregion

        #region FetchDataFromApi
        private async Task<List<T>> FetchDataFromApi<T>(string apiUrl)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(apiUrl);
            var responseData = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<T>>(responseData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        #endregion

        #region GetCitiesByState
        [HttpGet("GetCitiesByState")]
        public async Task<IActionResult> GetCitiesByState(int stateId)
        {

            List<CityListDTO> listCityListDTO = null;
            // Call your API to get cities by state
            var jsonPayload = JsonSerializer.Serialize(stateId);
            var client = _httpClientFactory.CreateClient();
            var apiUrl = "https://localhost:44398/api/GetCityByStateId";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                listCityListDTO = JsonSerializer.Deserialize<List<CityListDTO>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            //return Json(lstCityDTO);
            return Json(listCityListDTO);
        }
        #endregion GetCitiesByState

        [HttpPost("RegisterDoner")]
        public async Task<IActionResult> RegisterDoner(RequestRegisterDonerListDTO objRequestRegisterDonerListDTO)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewDataAsync();
                return View(objRequestRegisterDonerListDTO);
            }

            var validationErrors = new List<string>();

            if (!ModelState.IsValid)
            {
                validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewData["ValidationErrors"] = validationErrors;
                return View(objRequestRegisterDonerListDTO);
            }

            Response<string> objResponse = null;

            try
            {
                var jsonPayload = JsonSerializer.Serialize(objRequestRegisterDonerListDTO);
                var client = _httpClientFactory.CreateClient();
                var apiUrl = "https://localhost:44398/api/RegisterDoner";
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    objResponse = JsonSerializer.Deserialize<Response<string>>(responseData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (objResponse.StatusCode == 200)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", objResponse.Status);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error during registertion process. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred during the Registeration process.");
                return View("Error");
            }

            // Return the view with the model and validation errors

            validationErrors = ModelState.Values
       .SelectMany(v => v.Errors)
       .Select(e => e.ErrorMessage)
       .ToList();
            ViewData["ValidationErrors"] = validationErrors;

            // Return the view with the model and validation errors
            return View(objRequestRegisterDonerListDTO);
        }



        #endregion

        #region
        [HttpGet("Logout")]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
        #endregion
    }


}
