using Application.DataValidation.ExplicitWordList;
using Domain.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Moq;

namespace RealTimeKWhPrice.Test.DataValidationTests
{
    public class CheckForBadUserNamesTest
    {
        [Fact]
        [Trait("DataValidationTests", "CheckForBadUserNames")]
        public void CheckForBadUserNames_WithBadUserName_ShouldReturnFalse()
        {
            var mockLogger = A.Fake<ILogger<CheckForExplicitWord>>();

            var checkForExplicitWord = A.Fake<CheckForExplicitWord>(options =>
                options.WithArgumentsForConstructor(() => new CheckForExplicitWord("mock/path/to/explicitWords.json", mockLogger))
            );

            A.CallTo(() => checkForExplicitWord.CheckForBadWords(A<string>._))
                .Returns(OperationResult<bool>.Fail("Explicit word found", "ExplicitWordList"));

            var userName = "JohnIdiotDoe";
            var result = checkForExplicitWord.CheckForBadWords(userName);

            Assert.False(result.Succeeded);
            Assert.Equal("Explicit word found", result.ErrorMessage);
        }

    }
}
