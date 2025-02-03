using Application.DataValidation.ExplicitWordList;
using Application.DataValidation.DataSanitizer;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<User>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabse;
        private readonly DataSanitizerLogic _dataSanitizerLogic;
        private readonly ILogger<CheckForExplicitWord> _checkForExplicitWordLogger;

        public CreateUserCommandHandler(UserManager<User> userManager, ILogger<CreateUserCommandHandler> logger, ILoggerRepository loggerToDatabse, ILogger<CheckForExplicitWord> checkForExplicitWordLogger)
        {
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabse = loggerToDatabse;
            _dataSanitizerLogic = new DataSanitizerLogic();
            _checkForExplicitWordLogger = checkForExplicitWordLogger;
        }

        public async Task<OperationResult<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Received request to create user: {JsonConvert.SerializeObject(request.UserDto)}");

                // Validate and sanitize user data
                var (isUserNameValid, sanitizedUserName) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.UserName);
                var (isEmailValid, sanitizedEmail) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.Email);
                var (isPhoneNumberValid, sanitizedPhoneNumber) = _dataSanitizerLogic.ValidateAndSanitize(request.UserDto.PhoneNumber);

                _logger.LogInformation($"Sanitized UserName: {sanitizedUserName}, Email: {sanitizedEmail}, Phone: {sanitizedPhoneNumber}");

                // Validation checks
                if (!isUserNameValid)
                {
                    _logger.LogWarning($"User creation failed: Username '{sanitizedUserName}' contains forbidden words.");
                    return OperationResult<User>.Fail("UserName contains forbidden words.", "Application");
                }

                if (!isEmailValid)
                {
                    _logger.LogWarning($"User creation failed: Email '{sanitizedEmail}' contains forbidden words.");
                    return OperationResult<User>.Fail("Email contains forbidden words.", "Application");
                }

                if (!isPhoneNumberValid)
                {
                    _logger.LogWarning($"User creation failed: Phone number '{sanitizedPhoneNumber}' contains forbidden words.");
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

                _logger.LogInformation($"Checking explicit words for username: {createdUser.UserName}");

                var basePath = AppContext.BaseDirectory;
                var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Application", "DataValidation", "ExplicitWordList", "ExplicitWordsJson", "explicitWords.json");

                // Pass the injected logger here
                CheckForExplicitWord _checkForExplicitWord = new CheckForExplicitWord(jsonFilePath, _checkForExplicitWordLogger);

                var checkForBadWordsResult = _checkForExplicitWord.CheckForBadWords(createdUser.UserName);

                if (checkForBadWordsResult.Succeeded == false)
                {
                    _logger.LogWarning($"User creation blocked: {checkForBadWordsResult.ErrorMessage}");
                    return OperationResult<User>.Fail(checkForBadWordsResult.ErrorMessage, "Application");
                }

                _logger.LogInformation($"Creating user: {createdUser.UserName}, Email: {createdUser.Email}");

                var userCreationResult = await _userManager.CreateAsync(createdUser, request.UserDto.Password);

                if (!userCreationResult.Succeeded)
                {
                    var errors = string.Join(", ", userCreationResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when creating a user '{createdUser.UserName}': {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to create user: {errors}", "Application");
                }

                _logger.LogInformation($"User '{createdUser.UserName}' created successfully. Assigning role: {createdUser.Role}");

                var roleResult = await _userManager.AddToRoleAsync(createdUser, createdUser.Role.ToString());

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when assigning role to user '{createdUser.UserName}': {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to assign role: {errors}", "Application");
                }

                _logger.LogInformation($"User '{createdUser.UserName}' added successfully with role '{createdUser.Role}'.");

                return OperationResult<User>.Success(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"(Unexpected error). Error when creating user '{request.UserDto.UserName}': {ex.Message}");

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