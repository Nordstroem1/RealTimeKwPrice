using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElectricityPriceController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ElectricityPriceController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{region}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetElectricityPrices(string region)
        {
            var validRegions = new List<string> { "SE1", "SE2", "SE3", "SE4" };
            if (!validRegions.Contains(region.ToUpper()))
            {
                return BadRequest("Ogiltig region. Tillåtna regioner är: SE1 = Luleå / Norra Sverige, SE2 = Sundsvall / Norra Mellansverige, SE3 = Stockholm / Södra Mellansverige, SE4 = Malmö / Södra Sverige.");
            }

            var today = DateTime.Now;
            var url = $"https://www.elprisetjustnu.se/api/v1/prices/{today:yyyy}/{today:MM-dd}_{region.ToUpper()}.json";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var data = JsonSerializer.Deserialize<List<ElectricityPrice>>(content);
                return Ok(data);
            }
            catch (JsonException ex)
            {
                return BadRequest($"Error deserializing JSON: {ex.Message}");
            }
        }
    }
    }
