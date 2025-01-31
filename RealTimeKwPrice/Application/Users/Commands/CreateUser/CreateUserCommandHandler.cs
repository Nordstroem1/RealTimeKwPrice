using Application.DataValidation.ExplicitWordList;
using Application.DataValidation.DataSanitizer;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<User>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabse;
        private readonly DataSanitizerLogic _dataSanitizerLogic;

        public CreateUserCommandHandler(UserManager<User> userManager, ILogger<CreateUserCommandHandler> logger, ILoggerRepository loggerToDatabse)
        {
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabse = loggerToDatabse;
            _dataSanitizerLogic = new DataSanitizerLogic();
        }

        public async Task<OperationResult<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (isUserNameValid, sanitizedUserName) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.UserName);
                var (isEmailValid, sanitizedEmail) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.Email);
                var (isPhoneNumberValid, sanitizedPhoneNumber) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.PhoneNumber);

                _logger.LogInformation($"Sanitized UserName: {sanitizedUserName}");
                _logger.LogInformation($"Sanitized Email: {sanitizedEmail}");
                _logger.LogInformation($"Sanitized PhoneNumber: {sanitizedPhoneNumber}");

                if (!isUserNameValid)
                {
                    return OperationResult<User>.Fail("UserName contains forbidden words.", "Application");
                }

                if (!isEmailValid)
                {
                    return OperationResult<User>.Fail("Email contains forbidden words.", "Application");
                }

                if (!isPhoneNumberValid)
                {
                    return OperationResult<User>.Fail("Phone number contains forbidden words.", "Application");
                }

                var createdUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = sanitizedUserName,
                    Email = sanitizedEmail,       
                    PhoneNumber = sanitizedPhoneNumber, 
                    CreatedAt = request.UserDto.CreatedAt,
                    Role = RoleEnums.Roles.User,
                    Location = request.UserDto.Location,
                    PriceList = new List<ElectricityPrice>()
                };

                var basePath = AppContext.BaseDirectory;
                var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Application", "DataValidation", "ExplicitWordList", "ExplicitWordsJson", "explicitWords.json");
                CheckForExplicitWord _checkForExplicitWord = new CheckForExplicitWord(jsonFilePath);

                var checkForBadWordsResult = _checkForExplicitWord.CheckForBadWords(createdUser.UserName);

                if (checkForBadWordsResult.Succeeded == false)
                {
                    return OperationResult<User>.Fail(checkForBadWordsResult.ErrorMessage, "Application");
                }

                var userCreationResult = await _userManager.CreateAsync(createdUser, request.UserDto.Password);

                if (!userCreationResult.Succeeded)
                {
                    var errors = string.Join(", ", userCreationResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when creating a user: {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to create user: {errors}", "Application");
                }

                var roleResult = await _userManager.AddToRoleAsync(createdUser, createdUser.Role.ToString());

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when assigning role to user: {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to create user: {errors}", "Application");
                }

                _logger.LogInformation("User added successfully.");

                return OperationResult<User>.Success(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"(Unexpected error). Error when creating a user: {ex.Message}");

                await _loggerToDatabse.LogErrorAsync(new Logger
                {
                    Location = nameof(CreateUserCommandHandler),
                    WhatWentWrong = ex.Message,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                throw new Exception("Unexpected error." + ex.Message);
            }
        }
    }
}