using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services
{
    public class QrCodeService : IQrCodeService
    {
        public string GenerateQrCodeUri(string email, string unformattedKey, string issuer)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                UrlEncoder.Default.Encode(issuer),
                UrlEncoder.Default.Encode(email),
                unformattedKey);
        }
    }
    public interface IQrCodeService
    {
        string GenerateQrCodeUri(string email, string unformattedKey, string issuer);
    }
}
