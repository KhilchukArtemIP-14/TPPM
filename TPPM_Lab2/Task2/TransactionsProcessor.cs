using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TPPM_Lab2.Models;

namespace TPPM_Lab2.Task2
{
    public class TransactionsProcessor
    {
        public static async Task<double> ProcessPipeLineAsync(IEnumerable<Transaction> transactions, int maxThreads)
        {
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxThreads };

            var convertBlock = new TransformBlock<Transaction, Transaction>(tx =>
            {
                double convertedAmount = tx.Currency switch
                {
                    "USD" => tx.Amount * 40.0,
                    "EUR" => tx.Amount * 43.0,
                    _ => tx.Amount
                };
                return new Transaction() {UserId = tx.UserId, Date = tx.Date, ItemType = tx.ItemType, Amount = convertedAmount, Currency = "UAH" };
            }, options);


            var cashbackBlock = new TransformBlock<Transaction, Transaction>(tx =>
            {
                double finalAmount = tx.UserId % 5 == 0 ? tx.Amount * 0.8 : tx.Amount;
                return new Transaction() { UserId = tx.UserId, Date = tx.Date, ItemType = tx.ItemType, Amount = finalAmount, Currency = tx.Currency };
            }, options);


            double totalSum = 0;
            var aggregateBlock = new ActionBlock<Transaction>(tx =>
            {
                totalSum += tx.Amount;
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            convertBlock.LinkTo(cashbackBlock, linkOptions);
            cashbackBlock.LinkTo(aggregateBlock, linkOptions);

            foreach (var tx in transactions)
            {
                convertBlock.Post(tx);
            }

            convertBlock.Complete();
            await aggregateBlock.Completion;

            return totalSum;
        }
        public static async Task<double> ProcessProducerConsumerAsync(IEnumerable<Transaction> transactions, int maxThreads)
        {
            var channel = Channel.CreateBounded<Transaction>(new BoundedChannelOptions(10000)
            {
                SingleWriter = true,
                SingleReader = false
            });

            double globalSum = 0;
            object syncLock = new object();

            var producerTask = Task.Run(async () =>
            {
                foreach (var tx in transactions)
                {
                    await channel.Writer.WriteAsync(tx);
                }
                channel.Writer.Complete();
            });

            var consumerTasks = new Task[maxThreads];
            for (int i = 0; i < maxThreads; i++)
            {
                consumerTasks[i] = Task.Run(async () =>
                {
                    double localSum = 0;

                    await foreach (var tx in channel.Reader.ReadAllAsync())
                    {
                        double convertedAmount = tx.Currency switch
                        {
                            "USD" => tx.Amount * 40.0,
                            "EUR" => tx.Amount * 43.0,
                            _ => tx.Amount
                        };

                        double finalAmount = tx.UserId % 5 == 0 ? convertedAmount * 0.8 : convertedAmount;
                        localSum += finalAmount;
                    }

                    lock (syncLock)
                    {
                        globalSum += localSum;
                    }
                });
            }

            await Task.WhenAll(producerTask, Task.WhenAll(consumerTasks));

            return globalSum;
        }
    }
}
