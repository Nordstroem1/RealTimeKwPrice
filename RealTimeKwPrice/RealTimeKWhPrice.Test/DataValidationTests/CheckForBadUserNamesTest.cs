using Application.DataValidation.ExplicitWordList;

namespace RealTimeKWhPrice.Test.DataValidationTests
{
    public class CheckForBadUserNamesTest
    {
        [Fact]
        [Trait("DataValidationTests", "CheckForBadUserNames")]
        public void CheckForBadUserNames_WithBadUserName_ShouldReturnFalse()
        {
            //Arrange 
            var basePath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basePath, "ExplicitWordsJson", "explicitWords.json");
            var checkForExplicitWord = new CheckForExplicitWord(jsonFilePath);
            var userName = "JohnIdiotDoe";

            //Act
            var result = checkForExplicitWord.CheckForBadWords(userName);

            //Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Explicit word found", result.ErrorMessage);
        }
    }
}
