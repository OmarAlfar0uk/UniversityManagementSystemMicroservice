using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Auth.Contarcts;
using Microsoft.Extensions.Configuration;

namespace Auth.Repositories
{
    public class MailKitEmailService : IMailKitEmailService
    {
        private readonly IConfiguration _config;

        public MailKitEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            string subject = "Your OTP Code - Online Exam System";

            // Build HTML body
            string body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    body {{
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f9f9f9;
        color: #333;
        margin: 0;
        padding: 0;
    }}
    .email-container {{
        max-width: 600px;
        margin: 40px auto;
        background-color: #ffffff;
        border-radius: 10px;
        box-shadow: 0 5px 20px rgba(0,0,0,0.05);
        overflow: hidden;
    }}
    .header {{
        background-color: #0066ff;
        color: white;
        padding: 20px 30px;
        text-align: center;
        font-size: 24px;
        font-weight: bold;
    }}
    .content {{
        padding: 30px;
        text-align: center;
    }}
    .otp-box {{
        margin: 25px auto;
        display: inline-block;
        background-color: #f4f8ff;
        color: #0066ff;
        padding: 15px 30px;
        font-size: 32px;
        font-weight: bold;
        border-radius: 8px;
        letter-spacing: 5px;
        border: 2px solid #0066ff;
    }}
    .message {{
        font-size: 16px;
        color: #555;
        line-height: 1.6;
    }}
    .footer {{
        margin-top: 40px;
        font-size: 13px;
        color: #999;
        text-align: center;
        padding-bottom: 20px;
    }}
</style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            Online Exam System
        </div>
        <div class='content'>
            <p class='message'>Hello,</p>
            <p class='message'>
                We received a request to verify your identity. Use the OTP code below to continue:
            </p>

            <div class='otp-box'>{otpCode}</div>

            <p class='message'>
                This code will expire in <strong>5 minutes</strong>. 
                If you didn’t request this code, you can safely ignore this email.
            </p>
        </div>
        <div class='footer'>
            © {DateTime.UtcNow.Year} Online Exam System. All rights reserved.
        </div>
    </div>
</body>
</html>
";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendActivationEmailAsync(string toEmail, string userName, string activationCode, string role)
        {
            string subject = "Welcome to University Management System - Activate Your Account";

            // Build HTML body
            string body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    body {{
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f4f7f6;
        color: #333;
        margin: 0;
        padding: 0;
    }}
    .email-container {{
        max-width: 600px;
        margin: 40px auto;
        background-color: #ffffff;
        border-radius: 12px;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        overflow: hidden;
        border: 1px solid #e0e0e0;
    }}
    .header {{
        background: linear-gradient(135deg, #0044cc, #0066ff);
        color: white;
        padding: 30px 20px;
        text-align: center;
    }}
    .header h1 {{
        margin: 0;
        font-size: 24px;
        letter-spacing: 1px;
    }}
    .content {{
        padding: 40px 30px;
        line-height: 1.6;
    }}
    .welcome-text {{
        font-size: 18px;
        font-weight: 600;
        margin-bottom: 20px;
        color: #0044cc;
    }}
    .activation-box {{
        margin: 30px auto;
        padding: 20px;
        background-color: #f0f4ff;
        border: 2px dashed #0066ff;
        border-radius: 8px;
        text-align: center;
    }}
    .activation-code {{
        font-size: 32px;
        font-weight: bold;
        color: #0044cc;
        letter-spacing: 4px;
    }}
    .instruction {{
        font-size: 15px;
        color: #555;
        margin-top: 15px;
    }}
    .expiry-note {{
        font-size: 14px;
        color: #d9534f;
        font-weight: bold;
        text-align: center;
        margin-top: 10px;
    }}
    .footer {{
        background-color: #f9f9f9;
        padding: 20px;
        text-align: center;
        font-size: 12px;
        color: #888;
        border-top: 1px solid #eee;
    }}
</style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>University Management System</h1>
        </div>
        <div class='content'>
            <p class='welcome-text'>Hello {role} {userName},</p>
            <p>Welcome to our university! Your account has been successfully created by the administrator. To get started, please use the activation code below to activate your account.</p>

            <div class='activation-box'>
                <div class='activation-code'>{activationCode}</div>
                <p class='instruction'>Enter this code in the activation page</p>
            </div>
            <p class='expiry-note'>⚠️ This activation code will expire in 3 days.</p>

            <p>If you have any questions, please contact the IT support department.</p>
        </div>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} University Management System. All rights reserved.<br>
            This is an automated message, please do not reply.
        </div>
    </div>
</body>
</html>
";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["MailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["MailSettings:Host"],
                int.Parse(_config["MailSettings:Port"]),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(_config["MailSettings:Username"], _config["MailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
