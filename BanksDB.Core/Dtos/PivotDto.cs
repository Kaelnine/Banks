using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Dtos
{
        public class OrganizationAmountDto
        {
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; } = "";
            public decimal Amount { get; set; }
            public int TransactionsCount { get; set; }
        }

        public class PivotCellDto
        {
            public decimal Total { get; set; }
            public List<OrganizationAmountDto> Organizations { get; set; } = new();
        }

        public class PivotRowDto
        {
            public string Category { get; set; } = "";
            public string TransactionType { get; set; } = ""; // "Приход" | "Расход"
            public List<PivotCellDto> CellsByDay { get; set; } = new(); // индекс = день-1
            public decimal MonthTotal { get; set; }
        }

        public class PivotMonthDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int DaysInMonth { get; set; }
            public List<PivotRowDto> Rows { get; set; } = new();
            public List<PivotCellDto> DayTotals { get; set; } = new();
            public decimal TotalIncome { get; set; }
            public decimal TotalExpense { get; set; }
            public decimal GrandTotal => TotalIncome - TotalExpense; // сальдо
        }
}

