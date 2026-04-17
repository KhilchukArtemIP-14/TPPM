using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab2.Models;
using TPPM_Lab2.Task2;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TPPM_Lab2.Utils
{
    public static class TransactionGenerator
    {
        public static List<Transaction> Generate(int count)
        {
            var rnd = new Random(42);
            var transactions = new List<Transaction>(count);
            string[] currencies = { "UAH", "USD", "EUR" };
            string[] items = { "Groceries", "Electronics", "Subscription", "Clothing" };

            for (int i = 0; i < count; i++)
            {
                transactions.Add(new Transaction()
                {
                    UserId = rnd.Next(1, 100),
                    Amount = rnd.NextDouble() * 10000,
                    Currency = currencies[rnd.Next(currencies.Length)],
                    Date = DateTime.Now.AddDays(-rnd.Next(1, 365)),
                    ItemType = items[rnd.Next(items.Length)]
                });
            }
            return transactions;
        }
    }
}
