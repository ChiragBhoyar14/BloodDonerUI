using BloodBank.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BloodDoner.Controllers
{
    [Route("")]
    [Route("/")]
    [Route("Donor")]
    public class DonorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DonorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #region Available Doner

        [HttpGet("AvailableDoner")]
        public async Task<IActionResult> AvailableDoner(Request objRequest)
        {
            SearchAvailableBloodDonerListDTO objSearchAvailableBloodDonerListDTO = null;
            var validationErrors = new List<string>();
            Response<List<SearchAvailableBloodDonerListDTO>?> objResponse = null;


            try
            {
                // Retrieve the JWT token from session
                var jwtToken = HttpContext.Session.GetString("JWTToken");

                if (string.IsNullOrEmpty(jwtToken))
                {
                    ModelState.AddModelError("", "First need to login if you are already registered.");
                    
                }

                // Prepare request payload
                var jsonPayload = JsonSerializer.Serialize(objRequest); // Ensure correct object is serialized
                var client = _httpClientFactory.CreateClient();
                var apiUrl = "https://localhost:44398/api/SearchAvailableBloodDoner";
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Add JWT token to request headers
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                // Send POST request to API
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    objResponse = JsonSerializer.Deserialize<Response<List<SearchAvailableBloodDonerListDTO>>>(responseData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (objResponse.StatusCode == 200)
                    {
                            
                        return View(objResponse.Data);
                    }
                    else
                    {
                        ModelState.AddModelError("", objResponse.Status);
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error fetching available donors. API response: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View("Error");
            }

            // Collect validation errors
            validationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            ViewData["ValidationErrors"] = validationErrors;

            return View();  // Return view with error details
        }

        #endregion
    }

}
