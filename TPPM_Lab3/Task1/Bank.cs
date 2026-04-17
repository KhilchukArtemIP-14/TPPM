using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab3.Task1.TransferStrategies;

namespace TPPM_Lab3.Task1
{
    public class Bank
    {
        private readonly List<BankAccount> _accounts;
        private readonly Random _random = new Random();
        private readonly ITransferStrategy _strategy;
        public decimal TotalBalance => _accounts.Sum(a => a.Balance);

        public Bank(int accountCount, ITransferStrategy strategy)
        {
            _accounts = new List<BankAccount>();
            for (int i = 0; i < accountCount; i++)
            {
                _accounts.Add(new BankAccount(i, _random.Next(100, 10000)));
            }
            _strategy = strategy;
        }

        public async Task RunSimulationAsync(TimeSpan duration)
        {
            int numThreads = 1000;
            var tasks = new List<Task>();

            using var cts = new CancellationTokenSource(duration);

            for (int i = 0; i < numThreads; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        int from = _random.Next(_accounts.Count);
                        int to = _random.Next(_accounts.Count);

                        while (from == to) to = _random.Next(_accounts.Count);

                        decimal amount = _random.Next(10, 100);
                        var fromAccount = _accounts[from];
                        var toAccount = _accounts[to];

                        _strategy.Transfer(fromAccount, toAccount, amount);
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}