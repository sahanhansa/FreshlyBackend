using FreshlyBackendNew.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Text;

namespace FreshlyBackendNew.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var diagnosticInfo = new StringBuilder();
            
            try
            {
                // Gather diagnostic information
                diagnosticInfo.AppendLine($"=== EMAIL DIAGNOSTIC INFO ===");
                diagnosticInfo.AppendLine($"OS: {Environment.OSVersion}");
                diagnosticInfo.AppendLine($".NET Version: {Environment.Version}");
                diagnosticInfo.AppendLine($"Machine Name: {Environment.MachineName}");
                diagnosticInfo.AppendLine($"Current Directory: {Environment.CurrentDirectory}");
                
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"]);
                
                diagnosticInfo.AppendLine($"SMTP Server: {smtpServer}");
                diagnosticInfo.AppendLine($"SMTP Port: {port}");
                diagnosticInfo.AppendLine($"Username: {emailSettings["Username"]}");
                diagnosticInfo.AppendLine($"Target Email: {toEmail}");
                
                // Test network connectivity first
                await TestNetworkConnectivity(smtpServer, port, diagnosticInfo);
                
                // Test DNS resolution
                await TestDnsResolution(smtpServer, diagnosticInfo);
                
                Console.WriteLine(diagnosticInfo.ToString());
                
                // Try multiple approaches
                Exception lastException = null;
                
                // Approach 1: Standard SmtpClient with enhanced settings
                try
                {
                    diagnosticInfo.AppendLine("=== ATTEMPTING STANDARD SMTPCLIENT ===");
                    await SendWithStandardSmtp(emailSettings, toEmail, subject, body, isHtml, diagnosticInfo);
                    Console.WriteLine("SUCCESS: Standard SMTP approach worked!");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    diagnosticInfo.AppendLine($"Standard SMTP failed: {ex.Message}");
                    Console.WriteLine($"Standard SMTP failed: {ex.Message}");
                }
                
                // Approach 2: Try without SSL first, then upgrade
                try
                {
                    diagnosticInfo.AppendLine("=== ATTEMPTING STARTTLS APPROACH ===");
                    await SendWithStartTls(emailSettings, toEmail, subject, body, isHtml, diagnosticInfo);
                    Console.WriteLine("SUCCESS: STARTTLS approach worked!");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    diagnosticInfo.AppendLine($"STARTTLS failed: {ex.Message}");
                    Console.WriteLine($"STARTTLS failed: {ex.Message}");
                }
                
                // Approach 3: Try different security protocols
                try
                {
                    diagnosticInfo.AppendLine("=== ATTEMPTING DIFFERENT SECURITY PROTOCOLS ===");
                    await SendWithDifferentProtocols(emailSettings, toEmail, subject, body, isHtml, diagnosticInfo);
                    Console.WriteLine("SUCCESS: Different protocols approach worked!");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    diagnosticInfo.AppendLine($"Different protocols failed: {ex.Message}");
                    Console.WriteLine($"Different protocols failed: {ex.Message}");
                }
                
                // If all approaches fail, throw the last exception with full diagnostic info
                Console.WriteLine("=== ALL APPROACHES FAILED ===");
                Console.WriteLine(diagnosticInfo.ToString());
                throw new Exception($"All email sending approaches failed. Last error: {lastException?.Message}\n\nFull diagnostic info:\n{diagnosticInfo}", lastException);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error in SendEmailAsync: {ex.Message}");
                Console.WriteLine(diagnosticInfo.ToString());
                throw;
            }
        }
        
        private async Task TestNetworkConnectivity(string smtpServer, int port, StringBuilder diagnosticInfo)
        {
            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                var connectTask = client.ConnectAsync(smtpServer, port);
                var timeoutTask = Task.Delay(5000); // 5 second timeout
                
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                
                if (completedTask == timeoutTask)
                {
                    diagnosticInfo.AppendLine($"NETWORK: Connection to {smtpServer}:{port} timed out");
                }
                else if (connectTask.IsFaulted)
                {
                    diagnosticInfo.AppendLine($"NETWORK: Connection to {smtpServer}:{port} failed: {connectTask.Exception?.GetBaseException().Message}");
                }
                else
                {
                    diagnosticInfo.AppendLine($"NETWORK: Successfully connected to {smtpServer}:{port}");
                }
            }
            catch (Exception ex)
            {
                diagnosticInfo.AppendLine($"NETWORK: Error testing connectivity: {ex.Message}");
            }
        }
        
        private async Task TestDnsResolution(string smtpServer, StringBuilder diagnosticInfo)
        {
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(smtpServer);
                diagnosticInfo.AppendLine($"DNS: Resolved {smtpServer} to {string.Join(", ", hostEntry.AddressList.Select(ip => ip.ToString()))}");
            }
            catch (Exception ex)
            {
                diagnosticInfo.AppendLine($"DNS: Failed to resolve {smtpServer}: {ex.Message}");
            }
        }
        
        private async Task SendWithStandardSmtp(IConfigurationSection emailSettings, string toEmail, string subject, string body, bool isHtml, StringBuilder diagnosticInfo)
        {
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                diagnosticInfo.AppendLine("Standard SMTP: Email sent successfully");
            }
            finally
            {
                mailMessage?.Dispose();
                smtpClient?.Dispose();
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }
        
        private async Task SendWithStartTls(IConfigurationSection emailSettings, string toEmail, string subject, string body, bool isHtml, StringBuilder diagnosticInfo)
        {
            // Try connecting without SSL first, then upgrade
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = false, // Start without SSL
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                // Enable SSL after creating the client
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
                diagnosticInfo.AppendLine("STARTTLS: Email sent successfully");
            }
            finally
            {
                mailMessage?.Dispose();
                smtpClient?.Dispose();
            }
        }
        
        private async Task SendWithDifferentProtocols(IConfigurationSection emailSettings, string toEmail, string subject, string body, bool isHtml, StringBuilder diagnosticInfo)
        {
            // Try different security protocols
            var originalProtocol = ServicePointManager.SecurityProtocol;
            
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                
                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["Port"]),
                    Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                diagnosticInfo.AppendLine("Different Protocols: Email sent successfully");
                
                mailMessage?.Dispose();
                smtpClient?.Dispose();
            }
            finally
            {
                ServicePointManager.SecurityProtocol = originalProtocol;
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        public async Task<string> GetEmailTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(_environment.WebRootPath, "EmailTemplates", templateName);
            
            // Check if file exists (case-sensitive on macOS)
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found: {templatePath}");
            }
            
            return await File.ReadAllTextAsync(templatePath);
        }
    }
}