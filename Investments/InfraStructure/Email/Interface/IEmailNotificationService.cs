using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Email.Interface
{
    public interface IEmailNotificationService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
