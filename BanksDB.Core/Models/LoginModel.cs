using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Логин обязателен")]        
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(3, ErrorMessage = "Пароль должен содержать минимум 3 символов")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
