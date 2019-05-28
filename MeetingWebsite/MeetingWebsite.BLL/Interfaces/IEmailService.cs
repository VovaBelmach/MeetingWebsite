﻿using System.Threading.Tasks;

namespace MeetingWebsite.BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}