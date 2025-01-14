using Domain.Interfaces;
using FakeItEasy;
using MediatR;
using Domain.Models;
using Application.Users.Commands.CreateUser;
using Application.DTOs.User;

namespace RealTimeKWhPrice.Test.UserTests.Create
{
    public class CreateUserTest : TestBase<User>
    {
        [Fact]
        public async Task CreateUser_WithValidData_ShouldReturnUser()
        {
            //arrange
            CreateUserDto user = new CreateUserDto(
                userName: "User",
                email: "User@hotmail.com",
                password: "Password123141!#%&",
                phoneNumber: "1234567890",
                location: "Location"
            );
            var testResult = await Mediator.Send(new CreateUserCommand(user));
            A.CallTo(() => _Db.AddAsync(A<User>._)).Returns(testResult.Data);
        }
    }
}
