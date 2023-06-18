using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

public class MailSetting
{
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}

public class SendMailService : IEmailSender
{
    private readonly MailSetting _mailSetting;
    private readonly ILogger<SendMailService> _logger;
    public SendMailService(IOptions<MailSetting> mailSettings, ILogger<SendMailService> logger)
    {
        _mailSetting = mailSettings.Value;
        _logger = logger;
        _logger.LogInformation("Created SenmailService");
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
        message.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));

        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        // Thiết lập HTML gửi đi
        builder.HtmlBody = htmlMessage;
        // builder.Attachments => File đính kèm


        message.Body = builder.ToMessageBody();

        using var smtpClient = new SmtpClient();
        try
        {
            smtpClient.Connect(_mailSetting.Host, _mailSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_mailSetting.Mail, _mailSetting.Password);
            await smtpClient.SendAsync(message);
        }
        catch (Exception ex)
        {
            // nội dung mail thất bại lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailsavefile);

            _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            _logger.LogError(ex.Message);
        }

        smtpClient.Disconnect(true);
        _logger.LogInformation("send mail to " + email);
    }
}