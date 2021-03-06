﻿using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MeetingWebsite.BLL.Builders;
using MeetingWebsite.BLL.Infrastructure;
using MeetingWebsite.BLL.ViewModel;
using MeetingWebsite.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MeetingWebsite.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationSettings _applicationSettingsOption;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IUserProfileService _userProfileService;
        private const string ConfirmEmailController = "/api/account/ConfirmEmail";

        public AccountService(UserManager<User> userManager,
            IOptions<ApplicationSettings> applicationSettingsOption,
            IEmailService emailService,
            IFileService fileService,
            IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _applicationSettingsOption = applicationSettingsOption.Value;
            _emailService = emailService;
            _fileService = fileService;
            _userProfileService = userProfileService;
        }

        public async Task<object> RegisterUser(RegisterViewModel model, string url)
        {
            var user = model.CreateUser();
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return null;

            var userProfile = new UserProfile { UserId = user.Id };
            _userProfileService.CreateUserProfile(userProfile);

            await _emailService.SendEmailAsync(user.Email, Constants.ConfirmationEmail_Subject,
                string.Format(Constants.ConfirmationEmail_Message, CreateCallbackUrl(user, url).GetAwaiter().GetResult()));
            return result;
        }

        public async Task<object> UserForgotPassword(ForgotPasswordViewModel model, string url)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return null;
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encode = HttpUtility.UrlEncode(code);
            var callbackUrl = new StringBuilder("http://")
                .AppendFormat("localhost:4200/user/reset/")
                .AppendFormat($"{encode}");

            await _emailService.SendEmailAsync(user.Email, "Confirm your account",
                $"Confirm the registration by clicking on the " +
                $"link: <a href='{callbackUrl}'>link</a>");

            return user;
        }

        private async Task<StringBuilder> CreateCallbackUrl(User user, string url)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var urlParams = new Dictionary<string, string>
            {
                { "userId", user.Id },
                { "code", HttpUtility.UrlEncode(code) }
            };

            return new CallbackUrlBuilder().Build(url, ConfirmEmailController, urlParams);
        }

        public async Task<OperationDetails> ConfirmEmail(User user, string code)
        {
            var success = await _userManager.ConfirmEmailAsync(user, code);
            return new OperationDetails(success.Succeeded, success.Succeeded
                ? Constants.Status_Success : Constants.Status_Error, string.Empty);
        }

        public async Task<object> LoginUser(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) ||
                !await _userManager.IsEmailConfirmedAsync(user))
                return null;

            _fileService.SetUserFolder(user);
            return new JwtSecurityTokenBuilder()
                .Subject(user.Id).ExpiresInOneDay().SigningCredentials(_applicationSettingsOption.JWT_secret).Build();
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return null;
            }
            return await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordViewModel model, string userId)
        {
            var user = await GetUser(userId);
            if (user == null)
            {
                return null;
            }
            return await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
        }

        public async Task<User> GetUser(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}