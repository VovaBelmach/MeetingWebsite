﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using MeetingWebsite.BLL.ViewModel;
using MeetingWebsite.DAL.Interfaces;
using MeetingWebsite.Models.Entities;

namespace MeetingWebsite.BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork _database { get; set; }
        private IAccountService _accountService { get; set; }
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public UserService(IUnitOfWork database,
            Microsoft.AspNetCore.Identity.UserManager<User> userManager,
            IAccountService accountService)
        {
            _database = _database;
            _userManager = userManager;
            _accountService = accountService;
        }

        public async Task<EditUserProfileInformation> EditUserInformation(EditUserProfileInformation editUser)
        {
            var user = await _accountService.GetUser(editUser.Id);

            if (user == null || user.UserProfile == null)
                return null;

            var userProfile = user.UserProfile;

            try
            {
                if (!string.IsNullOrEmpty(editUser.FirstName))
                { user.FirstName = editUser.FirstName; }
                if (!string.IsNullOrEmpty(editUser.LastName))
                { user.LastName = editUser.LastName; }
                if (!string.IsNullOrEmpty(editUser.Genders.ToString()))
                { user.Gender = editUser.Genders; }
                if (!string.IsNullOrEmpty(editUser.Birthday.ToString(CultureInfo.InvariantCulture)))
                { user.Birthday = editUser.Birthday; }
                if (!string.IsNullOrEmpty(editUser.ZodiacSign.ToString()))
                { userProfile.ZodiacSign = editUser.ZodiacSign; }
                if (!string.IsNullOrEmpty(editUser.PurposeOfDating))
                { userProfile.PurposeOfDating = editUser.PurposeOfDating; }
                if (!string.IsNullOrEmpty(editUser.MaritalStatus))
                { userProfile.MaritalStatus = editUser.MaritalStatus; }
                if (!string.IsNullOrEmpty(editUser.Height))
                { userProfile.Height = editUser.Height; }
                if (!string.IsNullOrEmpty(editUser.Weight))
                { userProfile.Weight = editUser.Weight; }
                if (!string.IsNullOrEmpty(editUser.Education))
                { userProfile.Education = editUser.Education; }
                if (!string.IsNullOrEmpty(editUser.Nationality))
                { userProfile.Nationality = editUser.Nationality; }
                if (!string.IsNullOrEmpty(editUser.KnowledgeOfLanguages))
                { userProfile.KnowledgeOfLanguages = editUser.KnowledgeOfLanguages; }
                if (!string.IsNullOrEmpty(editUser.BadHabits))
                { userProfile.BadHabits = editUser.BadHabits; }
                if (!string.IsNullOrEmpty(editUser.FinancialSituation))
                { userProfile.FinancialSituation = editUser.FinancialSituation; }
                if (!string.IsNullOrEmpty(editUser.Interests))
                { userProfile.Interests = editUser.Interests; }
                user.AnonymityMode = editUser.AnonymityMode;

                await _userManager.UpdateAsync(user);

                var result = new EditUserProfileInformation(user);
                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}