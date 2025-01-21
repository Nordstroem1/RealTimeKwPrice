using Domain.Models;
using Newtonsoft.Json;

namespace Application.DataValidation.ExplicitWordList
{
    public class CheckForExplicitWord
    {
        private readonly string _jsonFilePath;
        public CheckForExplicitWord(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
        }
        public OperationResult<bool> CheckForBadWords(string userName)
        {
            try
            {
                List<string> ExplicitWords = new List<string>();
                if (string.IsNullOrEmpty(userName)) return OperationResult<bool>.Fail("The input was null or empty.", "ExplicitWordList");

                var basePath = AppContext.BaseDirectory;
                var totalFilePath = Path.Combine(basePath, _jsonFilePath);
                
                if (!File.Exists(totalFilePath))
                {
                    return OperationResult<bool>.Fail($"File not found: {totalFilePath}", "ExplicitWordList");
                }

                var jsonContent = File.ReadAllText(totalFilePath);
                var explicitWordsList = JsonConvert.DeserializeObject<List<string>>(jsonContent);
                
                if (explicitWordsList == null)
                {
                    return OperationResult<bool>.Fail("deserializedData was null", "ExplicitWordList");               
                }

                foreach (var word in explicitWordsList)
                {
                    if (userName.ToLower().Contains(word.ToLower()))
                    {
                        return OperationResult<bool>.Fail("Explicit word found", "ExplicitWordList");
                    }
                }

                return OperationResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while reading explicit words from json file", ex);
            }
        }
    }
}
