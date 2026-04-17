using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPPM_Lab2.Models;
using TPPM_Lab2.Task1;
using TPPM_Lab2.Utils;

namespace TPPM_Lab2.Task2
{
    public enum TransactionPatterns
    {
        Pipeline,
        ProducerConsumer
    }
    [MemoryDiagnoser]
    public class TransactionsBenchmarks
    {
        [ParamsAllValues]
        public TransactionPatterns Pattern { get; set; }
        public List<Transaction> Transactions { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Transactions = TransactionGenerator.Generate(1_000_000);
        }

        [Benchmark]
        public async Task<double> TransactionsBenchmark()
        {
            return Pattern switch
            {
                TransactionPatterns.Pipeline => await TransactionsProcessor.ProcessPipeLineAsync(Transactions,Environment.ProcessorCount),
                TransactionPatterns.ProducerConsumer => await TransactionsProcessor.ProcessProducerConsumerAsync(Transactions, Environment.ProcessorCount),
                _ => throw new Exception()
            };
        }
    }
}
