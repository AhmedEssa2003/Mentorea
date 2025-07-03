using MailKit.Net.Smtp;
using MailKit.Security;
using Mentorea.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Mentorea.Health
{
    public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
    {
        private readonly MailSettings _mailSettings = mailSettings.Value;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
			try
			{
                using var Smtp = new SmtpClient();

                Smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls,cancellationToken);
                Smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password,cancellationToken);
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
			catch (Exception exception)
			{

				return await Task.FromResult(HealthCheckResult.Unhealthy(exception:exception));
            }
        }
    }
}
