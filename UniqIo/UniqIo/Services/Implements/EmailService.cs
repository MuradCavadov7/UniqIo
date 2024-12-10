using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UniqIo.Helpers;
using UniqIo.Services.Interface;

namespace UniqIo.Services.Implements
{
    public class EmailService : IEmailService
    {
        readonly SmtpClient _smtpClient;
        readonly MailAddress _from;
        readonly HttpContext _context;

        public EmailService(IOptions<SmptOptions> option,IHttpContextAccessor accessor) 
        {
            var opt = option.Value;
            _smtpClient = new(opt.Host, opt.Port);
            _smtpClient.Credentials = new NetworkCredential(opt.Sender, opt.Password);
            _smtpClient.EnableSsl = true;
            _from = new MailAddress(opt.Sender, "UniqIo");
            _context = accessor.HttpContext;

        }

        public void SendEmailConfirmation(string reciever,string name,string token)
        {
            MailAddress to = new(reciever);
            MailMessage message = new MailMessage(_from, to);
            message.IsBodyHtml = true;
            message.Body = "Confirm your email adress";
            string url = _context.Request.Scheme + "://" + _context.Request.Host + "/Account/VerifyEmail?token=" + token + "&user=" + name;
            message.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
            _smtpClient.Send(message);

        }
    }
}
