using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models.InputModels
{
    public class DateRangeInputModel
    {
        [Required(ErrorMessage = "Дата начала обязательна")]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        [Required(ErrorMessage = "Дата окончания обязательна")]
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int AccountId { get; set; }
    }
}
