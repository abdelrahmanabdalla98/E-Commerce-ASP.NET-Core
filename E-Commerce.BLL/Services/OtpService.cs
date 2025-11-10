using E_Commerce.BLL.Models.Profile;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ISmsService _smsService;
        private readonly ILogger<OtpService> _logger;
        private const int OTP_EXPIRY_MINUTES = 5;
        private const int MAX_ATTEMPTS = 3;

        public OtpService(IMemoryCache cache, ISmsService smsService, ILogger<OtpService> logger)
        {
            _cache = cache;
            _smsService = smsService;
            _logger = logger;
        }

        public async Task<ValidationResultDTO> SendOtpAsync(string phoneNumber)
        {
            try
            {
                // Generate OTP
                var otpCode = GenerateOtp();
                var sessionId = Guid.NewGuid().ToString();

                // Create OTP session
                var otpSession = new OtpSessionDTO
                {
                    SessionId = sessionId,
                    PhoneNumber = phoneNumber,
                    OtpCode = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES),
                    AttemptCount = 0,
                    IsVerified = false
                };

                // Store in cache
                _cache.Set(sessionId, otpSession, TimeSpan.FromMinutes(OTP_EXPIRY_MINUTES));

                _logger.LogInformation($"OTP sent successfully to {phoneNumber}");
                return new ValidationResultDTO
                {
                    IsSuccess = true,
                    Message = otpCode,
                    SessionId = sessionId
                };
                // Send SMS
                //var smsResult = await _smsService.SendSmsAsync(phoneNumber, $"Your verification code is: {otpCode}. Valid for {OTP_EXPIRY_MINUTES} minutes.");

                //if (smsResult.IsSuccess)
                //{
                //    _logger.LogInformation($"OTP sent successfully to {phoneNumber}");
                //    return new ValidationResultDTO
                //    {
                //        IsSuccess = true,
                //        Message = "OTP sent successfully",
                //        SessionId = sessionId
                //    };
                //}
                //else
                //{
                //    _logger.LogError($"Failed to send OTP to {phoneNumber}: {smsResult.Message}");
                //    return new ValidationResultDTO
                //    {
                //        IsSuccess = false,
                //        Message = "Failed to send OTP. Please try again."
                //    };
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending OTP to {phoneNumber}");
                return new ValidationResultDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while sending OTP"
                };
            }
        }

        public ValidationResultDTO VerifyOtp(string sessionId, string phoneNumber, string otpCode)
        {
            try
            {
                if (!_cache.TryGetValue(sessionId, out OtpSessionDTO? session))
                {
                    return new ValidationResultDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid or expired session"
                    };
                }

                if (session.IsExpired)
                {
                    _cache.Remove(sessionId);
                    return new ValidationResultDTO
                    {
                        IsSuccess = false,
                        Message = "OTP has expired. Please request a new one."
                    };
                }

                if (session.PhoneNumber != phoneNumber)
                {
                    return new ValidationResultDTO
                    {
                        IsSuccess = false,
                        Message = "Phone number mismatch"
                    };
                }

                if (session.AttemptCount >= MAX_ATTEMPTS)
                {
                    _cache.Remove(sessionId);
                    return new ValidationResultDTO
                    {
                        IsSuccess = false,
                        Message = "Maximum verification attempts exceeded. Please request a new OTP."
                    };
                }

                session.AttemptCount++;
                _cache.Set(sessionId, session, TimeSpan.FromMinutes(OTP_EXPIRY_MINUTES));

                if (session.OtpCode == otpCode)
                {
                    session.IsVerified = true;
                    _cache.Set(sessionId, session, TimeSpan.FromMinutes(30)); // Extend for verified session

                    _logger.LogInformation($"OTP verified successfully for {phoneNumber}");
                    return new ValidationResultDTO
                    {
                        IsSuccess = true,
                        Message = "Phone number verified successfully",
                        SessionId = sessionId
                    };
                }
                else
                {
                    var remainingAttempts = MAX_ATTEMPTS - session.AttemptCount;
                    return new ValidationResultDTO
                    {
                        IsSuccess = false,
                        Message = $"Invalid OTP. {remainingAttempts} attempts remaining."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying OTP for {phoneNumber}");
                return new ValidationResultDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred during verification"
                };
            }
        }

        public async Task<bool> ResendOtpAsync(string sessionId)
        {
            try
            {
                if (!_cache.TryGetValue(sessionId, out OtpSessionDTO session))
                {
                    return false;
                }

                var result = await SendOtpAsync(session.PhoneNumber);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resending OTP for session {sessionId}");
                return false;
            }
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
    public interface IOtpService
    {
        Task<ValidationResultDTO> SendOtpAsync(string phoneNumber);
        ValidationResultDTO VerifyOtp(string sessionId, string phoneNumber, string otpCode);
        Task<bool> ResendOtpAsync(string sessionId);
    }
}
