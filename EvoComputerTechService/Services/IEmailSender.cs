using EvoComputerTechService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Services
{
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message);
    }
}
