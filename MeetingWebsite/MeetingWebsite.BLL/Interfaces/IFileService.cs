﻿using System.Threading.Tasks;
using MeetingWebsite.BLL.ViewModel;
using MeetingWebsite.Models.Entities;

namespace MeetingWebsite.BLL.Services
{
    public interface IFileService
    {
        void SetUserFolder(User user);
        Task AddUserAvatar(EditUserAvatarViewModel editAvatar);
    }
}