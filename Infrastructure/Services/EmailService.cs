using Application.Services;
using Domain.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private const string EndpointUrl = "https://send-emails-1021086131357.europe-west1.run.app";

        public EmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<string>> SendResetPasswordCodeEmailAsync(string toEmail, string code)
        {
            var html = $@"<!DOCTYPE html>
                        <html lang=""en"">
                        <head>
                          <meta charset=""UTF-8"" />
                          <title>Password Reset</title>
                          <style>
                            body {{
                              font-family: Arial, sans-serif;
                              color: #51423e;
                              text-align: center;
                              padding: 50px;
                            }}
                            .container {{
                              background-color: #efebe2;
                              padding: 25px 20px;
                              border-radius: 12px;
                              display: inline-block;
                              width: 90%;
                              max-width: 360px;
                            }}
                            .logo {{
                              width: 100%;
                              max-width: 350px;
                              border-radius: 10px 10px 0 0;
                              margin-bottom: 20px;
                            }}
                            .code {{
                              font-size: 36px;
                              font-weight: bold;
                              background: #ac9c94;
                              color: #544541;
                              padding: 15px 30px;
                              border-radius: 8px;
                              display: inline-block;
                              user-select: all;
                              margin: 20px 0;
                              letter-spacing: 6px;
                              font-family: 'Courier New', Courier, monospace;
                            }}
                            p {{
                              margin-top: 20px;
                              font-size: 16px;
                            }}
                          </style>
                        </head>
                        <body>
                          <div class=""container"">
                            <img src=""https://storage.googleapis.com/rewear/logo3.png"" alt=""Rewear Logo"" class=""logo"" />
                            <h2>Password Reset</h2>
                            <p>You requested a password reset for your Rewear account.</p>
                            <p>Please use the code below to reset your password:</p>
                            <div class=""code"">{code}</div>
                            <p>If you didn't request this, you can ignore this message.</p>
                            <p>The Rewear Team</p>
                          </div>
                        </body>
                        </html>";
            var subject = "Password Reset Code";

            var payload = new
            {
                to = toEmail,
                subject = subject,
                html = html
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(EndpointUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success("Email sent successfully");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure($"Failed to send email: {errorMessage}");
            }
        }


        public async Task<Result<string>> SendEmail(string toEmail, string subject, string body)
        {
            var payload = new
            {
                to = toEmail,
                subject = subject,
                html = body
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(EndpointUrl, content); 

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success("Email sent successfully");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure($"Failed to send email: {errorMessage}");
            }
        }
    }
}