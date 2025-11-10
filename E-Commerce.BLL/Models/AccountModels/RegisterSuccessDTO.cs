using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.AccountModels
{
    public class RegisterSuccessDTO
    {
        public string Email { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }
}
