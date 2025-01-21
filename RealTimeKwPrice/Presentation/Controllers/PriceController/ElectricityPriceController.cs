using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers.PriceController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElectricityPriceController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ElectricityPriceController> _logger; 
        private readonly ILoggerRepository _loggerToDatabase; 

        public ElectricityPriceController(HttpClient httpClient, ILogger<ElectricityPriceController> logger, ILoggerRepository loggerToDatabase)
        {
            _httpClient = httpClient;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpGet("{region}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetElectricityPrices(string region)
        {
            var validRegions = new List<string> { "SE1", "SE2", "SE3", "SE4" };
            if (!validRegions.Contains(region.ToUpper()))
            {
                var errorMessage = "Ogiltig region. Tillåtna regioner är: SE1 = Luleå / Norra Sverige, SE2 = Sundsvall / Norra Mellansverige, SE3 = Stockholm / Södra Mellansverige, SE4 = Malmö / Södra Sverige.";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(ElectricityPriceController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(GetElectricityPrices)
                });

                _logger.LogWarning(errorMessage);

                return BadRequest(errorMessage);
            }

            var today = DateTime.Now;
            var url = $"https://www.elprisetjustnu.se/api/v1/prices/{today:yyyy}/{today:MM-dd}_{region.ToUpper()}.json";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"Failed to retrieve electricity prices for region {region}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(ElectricityPriceController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(GetElectricityPrices)
                });

                _logger.LogError(errorMessage);

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
                var errorMessage = $"Error deserializing JSON: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(ElectricityPriceController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(GetElectricityPrices)
                });

                _logger.LogError(errorMessage);

                return BadRequest(errorMessage);
            }
        }
    }
}
