using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using PhoneNumber = Twilio.Types.PhoneNumber;
namespace E_Commerce.BLL.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<SmsResult> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var fromNumber = _configuration["Twilio:PhoneNumber"];

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                _logger.LogInformation($"SMS sent successfully. SID: {messageResource.Sid}");

                return new SmsResult
                {
                    IsSuccess = true,
                    Message = "SMS sent successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to {phoneNumber}");
                return new SmsResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
    public interface ISmsService
    {
        Task<SmsResult> SendSmsAsync(string phoneNumber, string message);
    }

    public class SmsResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
