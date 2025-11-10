using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class ShowRecoveryCodesDTO
    {
        public string[] RecoveryCodes { get; set; } = Array.Empty<string>();
        public string? ReturnUrl { get; set; }
    }
}
