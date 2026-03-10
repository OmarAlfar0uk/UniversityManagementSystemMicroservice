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

        public async Task SendActivationEmailAsync(string toEmail, string userName, string activationCode, string role, string universityId)
        {
            string subject = "Welcome to University Management System - Activate Your Account";

            string body = $@"<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    body {{
        font-family: 'Arial', 'Segoe UI', sans-serif;
        background-color: #f5f6f8;
        color: #000;
        margin: 0;
        padding: 0;
    }}
    .email-container {{
        max-width: 650px;
        margin: 40px auto;
        background-color: #ffffff;
        box-shadow: 0 8px 25px rgba(0, 102, 255, 0.15); /* Light blue shadow */
        border-radius: 8px;
        overflow: hidden;
    }}
    .header {{
        background-color: #0d6efd; /* Primary blue */
        color: #ffffff;
        text-align: center;
        padding: 25px 20px;
    }}
    .header h1 {{
        margin: 0;
        font-size: 24px;
        font-weight: bold;
        letter-spacing: 1px;
    }}
    .content {{
        padding: 40px 50px;
        box-sizing: border-box;
    }}
    .greeting {{
        font-size: 18px;
        font-weight: bold;
        color: #0d6efd;
        margin-bottom: 20px;
    }}
    .welcome-text {{
        font-size: 14px;
        line-height: 1.6;
        color: #333;
        margin-bottom: 30px;
    }}
    .info-box {{
        background-color: #f8faff;
        border: 1px dashed #0d6efd;
        border-radius: 6px;
        padding: 30px;
        margin-bottom: 30px;
        text-align: center;
    }}
    .label {{
        font-size: 14px;
        color: #555;
        margin-bottom: 10px;
    }}
    .value {{
        font-size: 36px;
        font-weight: bold;
        letter-spacing: 2px;
        margin: 0 0 10px 0;
    }}
    .value-black {{
        color: #333;
    }}
    .value-blue {{
        color: #0d6efd;
    }}
    .subtext {{
        font-size: 13px;
        color: #666;
        margin: 0;
    }}
    .divider {{
        height: 1px;
        background-color: #dcdfe6;
        margin: 25px 0;
    }}
    .warning-text {{
        color: #d9534f;
        text-align: center;
        font-size: 14px;
        margin-bottom: 25px;
    }}
    .support-text {{
        font-size: 14px;
        color: #333;
    }}
    .footer-container {{
        background-color: #f9f9f9;
        border-top: 1px solid #ebebeb;
        padding: 20px;
        text-align: center;
        font-size: 13px;
        color: #888;
        line-height: 1.5;
    }}
</style>
</head>
<body>
    <div style='background-color: #f5f6f8; padding: 20px 0;'>
        <div class='email-container'>
            <div class='header'>
                <h1>University Management System</h1>
            </div>

            <div class='content'>
                <div class='greeting'>
                    Hello {role} {userName},
                </div>
                
                <div class='welcome-text'>
                    Welcome to our university! Your account has been successfully created by the administrator.<br>
                    To get started, please use your <strong>University ID</strong> and the <strong>Activation Code</strong> below.
                </div>

                <div class='info-box'>
                    <div class='label'>Your University ID</div>
                    <div class='value value-black'>{universityId}</div>
                    <p class='subtext'>Use this ID along with your password to login later</p>

                    <div class='divider'></div>

                    <div class='label'>Your Activation Code</div>
                    <div class='value value-blue'>{activationCode}</div>
                    <p class='subtext'>Enter this code in the activation page</p>
                </div>

                <div class='warning-text'>
                    <span style='color: #ed9c28; font-weight: bold; margin-right: 5px;'>⚠️</span> This activation code will expire in 3 days.
                </div>

                <div class='support-text'>
                    If you have any questions, please contact the IT support department.
                </div>
            </div>
            
            <div class='footer-container'>
                &copy; {DateTime.UtcNow.Year} University Management System. All rights reserved.<br>
                This is an automated message, please do not reply.
            </div>
        </div>
    </div>
</body>
</html>";

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
