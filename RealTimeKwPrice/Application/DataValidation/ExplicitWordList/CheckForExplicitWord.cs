using Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.DataValidation.ExplicitWordList
{
    public class CheckForExplicitWord
    {
        private readonly string _jsonFilePath;
        private readonly ILogger<CheckForExplicitWord> _logger;

        // Injektera loggern via konstruktorn
        public CheckForExplicitWord(string jsonFilePath, ILogger<CheckForExplicitWord> logger)
        {
            _jsonFilePath = Path.GetFullPath(jsonFilePath);
            _logger = logger;
        }

        public OperationResult<bool> CheckForBadWords(string userName)
        {
            try
            {
                List<string> ExplicitWords = new List<string>();

                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("UserName is null or empty.");
                    return OperationResult<bool>.Fail("The input was null or empty.", "ExplicitWordList");
                }

                if (!File.Exists(_jsonFilePath))
                {
                    _logger.LogError($"File not found: {_jsonFilePath}");
                    return OperationResult<bool>.Fail($"File not found: {_jsonFilePath}", "ExplicitWordList");
                }

                // Läs JSON-filens innehåll
                var jsonContent = File.ReadAllText(_jsonFilePath);

                // Logga innehållet av JSON-filen
                _logger.LogInformation($"Explicit words JSON content: {jsonContent}");

                var explicitWordsList = JsonConvert.DeserializeObject<List<string>>(jsonContent);

                // Logga resultatet av deserialisering
                _logger.LogInformation($"Deserialized explicit words count: {explicitWordsList?.Count ?? 0}");

                if (explicitWordsList == null)
                {
                    _logger.LogError("Failed to deserialize explicit words list.");
                    return OperationResult<bool>.Fail("Deserialized data was null", "ExplicitWordList");
                }

                // Kolla om användarnamnet innehåller några förbjudna ord
                foreach (var word in explicitWordsList)
                {
                    // Dela upp användarnamnet i ord och kolla för exakt matchning
                    if (userName.Split(new[] { ' ', '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries)
                        .Any(part => part.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.LogWarning($"Explicit word found in username: {userName}");
                        return OperationResult<bool>.Fail("Explicit word found", "ExplicitWordList");
                    }
                }

                return OperationResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Logga om ett undantag inträffar
                _logger.LogError(ex, "Error while reading explicit words from JSON file.");
                throw new Exception("Error while reading explicit words from json file", ex);
            }
        }
    }
}
