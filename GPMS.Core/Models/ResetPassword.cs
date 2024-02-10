using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPMS.Core.Models
{
    public class ResetPassword
    {
        [Compare("Password", ErrorMessage = "Password and Confirmation Passowrd dont match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
