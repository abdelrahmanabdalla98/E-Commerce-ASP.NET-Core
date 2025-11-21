using E_Commerce.BLL.Services;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text;
using E_Commerce.BLL.Models.AccountModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Security.Cryptography;
using E_Commerce.DAL.DB_Context;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using YourApp.Services;
using Microsoft.AspNetCore.RateLimiting;
using E_Commerce.BLL.Models.ProfileModels;
using Microsoft.IdentityModel.Tokens;
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace E_Commerce.PL.Controllers
{
    [EnableRateLimiting("AccountPolicy")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailService emailService,
            IConfiguration configuration,
            IOtpService _otpService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
            this._otpService = _otpService;
        }
        // Add these methods to your AccountController.cs

        #region Phone Confirmation

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PhoneConfirmation(string? returnUrl = null)
        {
            var model = new PhoneDTO
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("PhonePolicy")]
        public IActionResult SendPhoneConfirmation(PhoneDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View("PhoneConfirmation", model);
            }

            try
            {
                // Generate 6-digit OTP
                var otp = GenerateOTP();

                // Store OTP in session/cache with expiration (5 minutes)
                var otpKey = $"phone_otp_{model.PhoneNumber}";
                HttpContext.Session.SetString(otpKey, otp);
                HttpContext.Session.SetString($"{otpKey}_timestamp", DateTime.UtcNow.ToString());

                // Here you would integrate with SMS service (Twilio, etc.)
                // await _smsService.SendOTPAsync(model.PhoneNumber, otp);

                // For demo purposes, log the OTP (remove in production)
                _logger.LogInformation("OTP for {PhoneNumber}: {OTP}", model.PhoneNumber, otp);

                var verifyModel = new PhoneDTO
                {
                    PhoneNumber = model.PhoneNumber,
                    ReturnUrl = model.ReturnUrl
                };

                TempData["StatusMessage"] = "OTP sent successfully! Please check your phone.";
                return View("VerifyPhoneOtp", verifyModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {PhoneNumber}", model.PhoneNumber);
                ModelState.AddModelError(string.Empty, "Failed to send OTP. Please try again.");
                return View("PhoneConfirmation", model);
            }
        }
        [HttpGet]
        public  IActionResult VerifyPhoneOtp()
        {
            return View(new PhoneDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("PhonePolicy")]
        public async Task<IActionResult> VerifyPhoneOtp(PhoneDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var otpKey = $"phone_otp_{model.PhoneNumber}";
                var timestampKey = $"{otpKey}_timestamp";

                var storedOtp = HttpContext.Session.GetString(otpKey);
                var timestampString = HttpContext.Session.GetString(timestampKey);

                if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(timestampString))
                {
                    ModelState.AddModelError(string.Empty, "OTP has expired. Please request a new one.");
                    return View(model);
                }

                if (!DateTime.TryParse(timestampString, out var timestamp))
                {
                    ModelState.AddModelError(string.Empty, "Invalid OTP session. Please try again.");
                    return View(model);
                }

                // Check if OTP is expired (5 minutes)
                if (DateTime.UtcNow.Subtract(timestamp).TotalMinutes > 5)
                {
                    HttpContext.Session.Remove(otpKey);
                    HttpContext.Session.Remove(timestampKey);
                    ModelState.AddModelError(string.Empty, "OTP has expired. Please request a new one.");
                    return View(model);
                }

                if (storedOtp != model.OtpCode)
                {
                    ModelState.AddModelError("OtpCode", "Invalid OTP code. Please try again.");
                    return View(model);
                }

                // OTP verified successfully
                HttpContext.Session.Remove(otpKey);
                HttpContext.Session.Remove(timestampKey);

                // Here you can update user's phone confirmation status if needed
                // For example, if user is logged in:
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        user.PhoneNumber = model.PhoneNumber;
                        user.PhoneNumberConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                }

                _logger.LogInformation("Phone number {PhoneNumber} verified successfully", model.PhoneNumber);

                var successModel = new PhoneDTO
                {
                    PhoneNumber = model.PhoneNumber,
                    ReturnUrl = model.ReturnUrl
                };

                return View("PhoneConfirmed", successModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {PhoneNumber}", model.PhoneNumber);
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("PhonePolicy")]
        public IActionResult ResendPhoneOtp(PhoneDTO model)
        {
            try
            {
                // Generate new OTP
                var otp = GenerateOTP();

                // Store new OTP
                var otpKey = $"phone_otp_{model.PhoneNumber}";
                HttpContext.Session.SetString(otpKey, otp);
                HttpContext.Session.SetString($"{otpKey}_timestamp", DateTime.UtcNow.ToString());

                // Send SMS (integrate with your SMS service)
                // await _smsService.SendOTPAsync(model.PhoneNumber, otp);

                _logger.LogInformation("OTP resent for {PhoneNumber}: {OTP}", model.PhoneNumber, otp);

                TempData["StatusMessage"] = "New OTP sent successfully!";
                return View("VerifyPhoneOtp", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to {PhoneNumber}", model.PhoneNumber);
                TempData["ErrorMessage"] = "Failed to resend OTP. Please try again.";
                return View("VerifyPhoneOtp", model);
            }
        }

        #endregion

        #region Private Helper Methods (add to existing helper methods section)

        private static string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        #endregion
        #region Registration

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Home", "Base");
            }

            var model = new RegisterDTO
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Home", "Base");
            }

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName?.Trim(),
                    LastName = model.LastName?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} created a new account", user.Email);

                    // Send email confirmation
                    await SendEmailConfirmationAsync(user);

                    // Add user to default role if specified
                    var defaultRole = _configuration["Identity:DefaultRole"];
                    if (!string.IsNullOrEmpty(defaultRole))
                    {
                        await _userManager.AddToRoleAsync(user, defaultRole);
                    }

                    var successModel = new RegisterSuccessDTO
                    {
                        Email = user.Email,
                        ReturnUrl = model.ReturnUrl
                    };

                    return View("RegisterSuccess", successModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning("Registration failed for {Email}: {Error}", model.Email, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            }

            return View(model);
        }

        #endregion

        #region Login

        [HttpGet]
        [AllowAnonymous]

        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            var model = new LoginDTO
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("AccountPolicy")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent user: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                    return View(model);
                }

                // Check if account is locked
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Login attempt for locked account: {Email}", model.Email);
                    return RedirectToAction(nameof(Lockout));
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Update last login time
                    user.LastLoginAt = DateTime.UtcNow;
                    user.LoginHistory.Add(DateTime.UtcNow);
                    await _userManager.UpdateAsync(user);
                    if (user.IsActive)
                    {
                        //await _emailService.SendEmailAsync(user.Email, "Last Log In", user.LastLoginAt.ToString());
                    }
                    _logger.LogInformation("User {Email} logged in successfully", user.Email);
                    return RedirectToLocal(model.ReturnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(TwoFactorAuthentication),
                        new { returnUrl = model.ReturnUrl, rememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} account locked out", user.Email);
                    return RedirectToAction(nameof(Lockout));
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Login not allowed. Please confirm your email.");
                    return View(model);
                }

                _logger.LogWarning("Invalid login attempt for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            }

            return View(model);
        }

        #endregion

        #region Two-Factor Authentication

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorAuthentication(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogError("Unable to load two-factor authentication user");
                return RedirectToAction(nameof(Login));
            }

            var model = new TwoFactorDTO
            {
                RememberMachine = rememberMe,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("TwoFactorPolicy")]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    _logger.LogError("Unable to load two-factor authentication user");
                    return RedirectToAction(nameof(Login));
                }

                var authenticatorCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                    authenticatorCode, model.RememberMachine, model.RememberMachine);

                if (result.Succeeded)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User {Email} logged in with 2FA", user.Email);
                    return RedirectToLocal(model.ReturnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} locked out during 2FA", user.Email);
                    return RedirectToAction(nameof(Lockout));
                }

                _logger.LogWarning("Invalid 2FA code for user {Email}", user.Email);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during two-factor authentication");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactor(string url)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.TwoFactorEnabled)
            {
                if (url != null)
                {
                    return Redirect(url);
                }
                else
                {
                    return RedirectToAction("Home", "Base");
                }
            }

            var model = new EnableTwoFactorDTO()
            {
                ReturnUrl = url
            };
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactor(EnableTwoFactorDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            try
            {
                var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

                if (!is2faTokenValid)
                {
                    ModelState.AddModelError("Code", "Verification code is invalid.");
                    await LoadSharedKeyAndQrCodeUriAsync(user, model);
                    return View(model);
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
                var userId = await _userManager.GetUserIdAsync(user);

                _logger.LogInformation("User {UserId} enabled 2FA with authenticator app", userId);

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 0);

                if (await _userManager.CountRecoveryCodesAsync(user) == 0)
                {
                    recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                    var recoveryModel = new ShowRecoveryCodesDTO
                    {
                        RecoveryCodes = recoveryCodes?.ToArray() ?? Array.Empty<string>(),
                        ReturnUrl = model.ReturnUrl
                    };
                    return View("ShowRecoveryCodes", recoveryModel);
                }
                if (model.ReturnUrl != null)
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Home", "Base");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling 2FA for user {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while enabling 2FA. Please try again.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }
        }

        #endregion

        #region Email Confirmation

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid email confirmation link.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Email confirmed for user {Email}", user.Email);
                    var model = new EmailSentDTO
                    {
                        Email = user.Email ?? ""
                    };
                    return View("EmailConfirmed", model);
                }

                _logger.LogWarning("Email confirmation failed for user {Email}", user.Email);
                return View("EmailConfirmationFailed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation for user {UserId}", userId);
                return View("EmailConfirmationFailed");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
            return View(new ResendEmailConfirmationDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("EmailPolicy")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    var successModel = new EmailSentDTO
                    {
                        Email = model.Email,
                        Message = "If an account with that email exists, a confirmation email has been sent."
                    };
                    return View("EmailSent", successModel);
                }

                if (user.EmailConfirmed)
                {
                    var alreadyConfirmedModel = new EmailSentDTO
                    {
                        Email = model.Email,
                        Message = "This email is already confirmed."
                    };
                    return View("EmailSent", alreadyConfirmedModel);
                }

                await SendEmailConfirmationAsync(user);
                var sentModel = new EmailSentDTO
                {
                    Email = model.Email,
                    Message = "Confirmation email sent. Please check your email."
                };
                return View("EmailSent", sentModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending email confirmation for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        #endregion

        #region Password Reset

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [EnableRateLimiting("EmailPolicy")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account",
                    new { userId = user.Id, code }, Request.Scheme);

                await _emailService.SendPasswordResetAsync(user.Email!, callbackUrl!);

                _logger.LogInformation("Password reset email sent for user {Email}", user.Email);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            var model = new ResetPasswordDTO { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for user {Email}", user.Email);
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
            }

            return View(model);
        }

        [HttpGet("reset-password-confirmation")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _signInManager.SignOutAsync();

                _logger.LogInformation("User {UserId} logged out", userId);
                TempData["StatusMessage"] = "You have been logged out successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }

            return RedirectToAction("Home", "Base");
        }

        #endregion

        #region Account Status

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet("access-denied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region Private Helper Methods

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableTwoFactorDTO model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey!);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email!, unformattedKey!);
        }

        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;

            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var appName = _configuration["Identity:ApplicationName"] ?? "AuthApp";

            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                UrlEncoder.Default.Encode(appName),
                UrlEncoder.Default.Encode(email),
                unformattedKey);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                if (Request.Method == "POST")
                {
                    return RedirectToAction("Home","Base");
                } 
                return Redirect(returnUrl);
            }

            return RedirectToAction("Home", "Base");
        }

        private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = user.Id, code }, Request.Scheme);
            using (var write = new StreamWriter(@"C:\Users\abdelrahman.abdalla\Desktop\Test\test5.text"))
            {
                await write.WriteLineAsync(callbackUrl);
            }
            //await _emailService.SendEmailConfirmationAsync(user.Email!, callbackUrl!);
        }
        [HttpGet]
        public async Task<IActionResult> RecoveryForm()
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id != null)
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    var model = new RecoveryDTO();
                    model.RecovEmail = user.RecovEmail;
                    model.RecovPhone = user.RecovPhone;
                    if(user.PassDictionaryJson!= null)
                    {
                        model.Answers = JsonConvert.DeserializeObject<List<QA>>(user.PassDictionaryJson);
                    }
                    return View(model);
                }
            }
            return View(new RecoveryDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RecoveryForm(RecoveryDTO model)
        {
            if (model.FormName == "email")
            {
                if (ModelState.TryGetValue("Email", out var entry) && entry.Errors.Count == 0)
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (!model.OtpCode.IsNullOrEmpty())
                    {
                        model.Step = 3;
                        if (model.CountDown <= 0)
                        {

                            if (model.IsReset)
                            {
                                model.CountDown = 300;
                            }
                            else
                            {
                                ModelState.AddModelError("CountDown", "Code Expired Please Resent Another One");

                            }
                        }
                        else
                        {
                                model.otpObject = _otpService.VerifyOtp(model.otpObject.SessionId, model.Email, model.OtpCode);
                                if (model.otpObject.IsSuccess == false)
                                {
                                    ModelState.AddModelError("CountDown", "OTP is Invalid Please Try Again");
                                }
                                else
                                {
                                    user.RecovEmail = model.Email;
                                    await _userManager.UpdateAsync(user);
                                }
                            
                        }
                    }
                    else
                    {
                        model.otpObject = await _otpService.SendOtpAsync(model.Email);
                        using (var writer = new StreamWriter(@"C:\Users\abdelrahman.abdalla\Desktop\Test\test6.text"))
                        {
                            writer.WriteLine("{0},{1},{2}", model.otpObject.IsSuccess, model.otpObject.Message, model.otpObject.SessionId);
                        }
                        model.Step = 2;
                        model.CountDown = 300;

                    }
                }

            }
            else if (model.FormName == "phone")
            {
                if (ModelState.TryGetValue("PhoneNumber", out var phone) && phone.Errors.Count == 0)
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (!model.OtpCode.IsNullOrEmpty())
                    {
                        model.Step = 3;
                        if (model.CountDown <= 0)
                        {

                            if (model.IsReset)
                            {
                                model.CountDown = 300;
                            }
                            else
                            {
                                ModelState.AddModelError("CountDown", "Code Expired Please Resent Another One");

                            }
                        }
                        else
                        {
                                model.otpObject = _otpService.VerifyOtp(model.otpObject.SessionId, model.PhoneNumber, model.OtpCode);
                                if (model.otpObject.IsSuccess == false)
                                {
                                    ModelState.AddModelError("CountDown", "OTP is Invalid Please Try Again");
                                }
                                else
                                {
                                    user.RecovPhone = model.PhoneNumber;
                                    await _userManager.UpdateAsync(user);
                                }
                            
                        }
                    }
                    else
                    {
                        model.otpObject = await _otpService.SendOtpAsync(model.PhoneNumber);
                        using (var writer = new StreamWriter(@"C:\Users\abdelrahman.abdalla\Desktop\Test\test6.text"))
                        {
                            writer.WriteLine("{0},{1},{2}", model.otpObject.IsSuccess, model.otpObject.Message, model.otpObject.SessionId);
                        }
                        model.Step = 2;
                        model.CountDown = 300;
                    }

                }

            }
            else
                {
                if (ModelState.Keys.Where(k => k.StartsWith("Answers")).All(k => !ModelState[k].Errors.Any()))
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        user.PassDictionaryJson = JsonConvert.SerializeObject(model.Answers, Formatting.Indented);
                        try
                        {
                            var myConcurrentDict = await _userManager.UpdateAsync(user);
                            if (myConcurrentDict.Succeeded)
                            {
                                model.Step = 4;
                            }
                        }
                        catch (Exception e)
                        {
                            await Console.Out.WriteLineAsync(e.Message);
                        }
                    }

                }
                else
                {
                    ViewBag.fail = true;
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RecoveryOpts(string type, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (type == "email")
                {
                    user.RecovEmail = null;
                    var ret = await _userManager.UpdateAsync(user);
                    if (ret.Succeeded)
                    {
                        return Json(new { respose = true });
                    }
                }
                else if (type == "phone")
                {
                    user.RecovPhone = null;
                    var ret = await _userManager.UpdateAsync(user);
                    if (ret.Succeeded)
                    {
                        return Json(new { respose = true });
                    }
                }
                else
                {
                    user.PassDictionaryJson = null;
                    var ret = await _userManager.UpdateAsync(user);
                    if (ret.Succeeded)
                    {
                        return Json(new { respose = true });
                    }
                }
            }
            return Json(new {res = false});
        }

        #endregion
    }
}

