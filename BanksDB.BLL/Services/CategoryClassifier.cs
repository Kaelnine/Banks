using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public interface ICategoryClassifier
    {
        string Classify(string? description, string transactionType, string counterparty);
    }

    public class KeywordCategoryClassifier : ICategoryClassifier
    {
        // Порядок важен: первое совпадение выигрывает.
        // Если понадобится — вынесу это в БД (таблица CategoryRule).
        private static readonly (string Category, string[] Keywords)[] Rules = new[]
        {
            ("НДФЛ",                 new[] { "ндфл" }),
            ("Страховые взносы",     new[] { "страховые взносы", "страх. взнос" }),
            ("Зарплата",             new[] { "зарплат", "денежное вознаграждение", "аванс", "реестр" }),
            ("Аренда",               new[] { "аренд" }),
            ("Налоги",               new[] { "налог", "усн", "енвд", "патент" }),
            ("Банковские комиссии",  new[] { "комисси", "ведение счета", "обслуживание счета" }),
            ("Внутренний перевод",   new[] { "перевод средств между счетами" }),
            ("Услуги",               new[] { "оказание услуг", "за оказание", "по счету", "по сч." }),
        };

        public string Classify(string? description, string transactionType, string? counterparty)
        {
            var text = $"{description} {counterparty}".ToLowerInvariant();
            foreach (var (category, keywords) in Rules)
                if (keywords.Any(k => text.Contains(k)))
                    return category;

            return transactionType == "Приход" ? "Прочие поступления" : "Прочие списания";
        }
    }
}
