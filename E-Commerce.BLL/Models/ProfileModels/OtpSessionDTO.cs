using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.Profile
{
    public class OtpSessionDTO
    {
        public string SessionId { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int AttemptCount { get; set; }
        public bool IsVerified { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
