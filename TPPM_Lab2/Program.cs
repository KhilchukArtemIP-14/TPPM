using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TPPM_Lab2.Task1;
using TPPM_Lab2.Task2;
using TPPM_Lab2.Utils;

namespace TPPM_Lab2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Would you like to run Task1 benchmarks? y/n");
            var key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<Task1Benchmarks>(config, args);
            }

            Console.WriteLine("Would you like to run Task1 benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<TransactionsBenchmarks>(config, args);
            }

            int threads = Environment.ProcessorCount;

            Console.WriteLine("--- Array Statistics (1_000_000) ---");
            Console.WriteLine("Generating array...");
            var rnd = new Random();
            int[] numbers = new int[1_000_000];
            for (int i = 0; i < numbers.Length; i++) numbers[i] = rnd.Next(0, 100_000);

            var statsMR = ArrayStatistics.GetStatsMapReduce(numbers, threads);
            var statsWP = ArrayStatistics.GetStatsWorkerPool(numbers, threads);
            var statsFJ = await ArrayStatistics.GetStatsForkJoinAsync(numbers, 10000);

            Console.WriteLine($"MapReduce - Min: {statsMR.Min}, Max: {statsMR.Max}, Avg: {statsMR.Average:F2}, Median: {statsMR.Median}");
            Console.WriteLine($"WorkerPool - Min: {statsWP.Min}, Max: {statsWP.Max}, Avg: {statsWP.Average:F2}, Median: {statsWP.Median}");
            Console.WriteLine($"ForkJoin - Min: {statsFJ.Min}, Max: {statsFJ.Max}, Avg: {statsFJ.Average:F2}, Median: {statsFJ.Median}");

            Console.WriteLine("\n--- TASK 1: HTML Tag Counter (1,000 documents) ---");
            Console.WriteLine("Generating HTML tree...");
            var htmlDocs = HtmlGenerator.GenerateDocumentsAndSave(1000, "HtmlConcurrencyFiles", 5, 4);

            var tagsMR = HtmlTagCounter.CountTagsMapReduce(htmlDocs, threads);
            var tagsWP = HtmlTagCounter.CountTagsWorkerPool(htmlDocs, threads);
            var tagsFJ = await HtmlTagCounter.CountTagsForkJoinAsync(htmlDocs);

            Console.WriteLine($"Total unique tags found: {tagsMR.Count}");
            Console.WriteLine($"MapReduce - <div> count: {tagsMR.GetValueOrDefault("div", 0)}");
            Console.WriteLine($"WorkerPool - <div> count: {tagsWP.GetValueOrDefault("div", 0)}");
            Console.WriteLine($"ForkJoin - <div> count: {tagsFJ.GetValueOrDefault("div", 0)}");

            Console.WriteLine("\n--- Matrix Multiplication (1000x1000) ---");
            int matrixSize = 1000;
            double[,] matrixA = new double[matrixSize, matrixSize];
            double[,] matrixB = new double[matrixSize, matrixSize];

            for (int i = 0; i < matrixSize; i++)
                for (int j = 0; j < matrixSize; j++)
                {
                    matrixA[i, j] = rnd.NextDouble();
                    matrixB[i, j] = rnd.NextDouble();
                }

            Console.WriteLine("Multiplying matrices...");
            var matMR = MatrixMultiplier.MultiplyMapReduce(matrixA, matrixB, matrixSize, threads);
            var matWP = MatrixMultiplier.MultiplyWorkerPool(matrixA, matrixB, matrixSize, threads);
            var matFJ = await MatrixMultiplier.MultiplyForkJoinAsync(matrixA, matrixB, matrixSize, 64);

            int checkRow = 250, checkCol = 250;
            Console.WriteLine($"MapReduce - Cell [{checkRow},{checkCol}]: {matMR[checkRow, checkCol]:F4}");
            Console.WriteLine($"WorkerPool - Cell [{checkRow},{checkCol}]: {matWP[checkRow, checkCol]:F4}");
            Console.WriteLine($"ForkJoin - Cell [{checkRow},{checkCol}]: {matFJ[checkRow, checkCol]:F4}");


            Console.WriteLine("\n--- Transactions (500_000) ---");
            Console.WriteLine("Generating transactions...");
            var transactions = TransactionGenerator.Generate(500_000);

            Console.WriteLine("Processing streams...");
            double sumPipeline = await TransactionsProcessor.ProcessPipeLineAsync(transactions, threads);
            double sumProducerConsumer = await TransactionsProcessor.ProcessProducerConsumerAsync(transactions, threads);

            Console.WriteLine($"Pipeline - {sumPipeline:N2} UAH");
            Console.WriteLine($"Producer-consumer - {sumProducerConsumer:N2} UAH");
        }
    }
}
