using MarketTradeApplication.Models.Email;
using MarketTradeApplication.Repositories.Interface;
using System.Net;
using System.Net.Mail;

namespace MarketTradeApplication.Repositories.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        public async Task<bool> EmailSendAsync(string email, string subject, string message)
        {
            bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKey = configuration.GetValue<string>("AppSettings:SecretKey"),
                    From = configuration.GetValue<string>("AppSettings:EmailSettings:From"),
                    SmtpServer = configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer"),
                    Port = configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL"),
                };

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject= subject,
                    Body = message,
                    BodyEncoding = System.Text.Encoding.ASCII,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);
                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new NetworkCredential(getEmailSetting.From,getEmailSetting.SecretKey),
                    EnableSsl= getEmailSetting.EnableSSL
                };
                await smtpClient.SendMailAsync(mailMessage);
                status = true;



            }
            catch (Exception)
            { 
                status= false;
            }
            return status;
        }
    }
}
