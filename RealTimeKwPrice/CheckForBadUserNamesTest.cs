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
            var basepath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basepath, "DataValidaton/ExplicitWordList/ExplicitWordsJson/explicitWords.json");
        }
    }
}
