using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TPPM.CpuBound;
using TPPM.IoBound;
using TPPM.MemoryBound;
using TPPM.Utils;

namespace TPPM
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Would you like to run CPU bound benchmarks? y/n");
            var key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<CpuBoundBenchmarks>(config, args);
            }

            Console.WriteLine("Would you like to run Memory Bound benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<MemoryBoundBenchmarks>(config, args);
            }

            Console.WriteLine("Would you like to run IO bound benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<IoBoundBenchmarks>(config, args);
            }

            int threads = 12;
            Console.WriteLine($"=== Functional Verification (Threads: {threads}) ===\n");

            //--- CPU BOUND ---
            var cpu = new CpuBoundTasks();

            Console.WriteLine("--- CPU: Monte-Carlo pi (1mil dots)---");
            double pi = cpu.CalculatePiMonteCarlo(1_000_000, threads);
            Console.WriteLine($"Result: {pi:F5}");

            Console.WriteLine("\n--- CPU: primes (0 to 100) ---");
            var primes = cpu.FindPrimesInRange(0, 100, threads);
            primes.Sort();
            Console.WriteLine($"Found {primes.Count} primes: {string.Join(", ", primes)}");

            Console.WriteLine("\n--- CPU: factorization (for 4950) ---");
            var factors = cpu.FactorizeNumber(4950, threads);
            factors.Sort();
            Console.WriteLine($"Factors: {string.Join(", ", factors)}");


            //--- MEMORY BOUND ---

            Console.WriteLine("\n--- Memory: Matrix Transpose (5x5) ---");
            var mem = new MemoryBoundTasks();

            int size = 5;
            var matrix = MatrixGenerator.GenerateMatrix(size);

            int sampleRow = 0, sampleCol = 4;
            int originalValue = matrix[sampleRow, sampleCol];
            Console.WriteLine($"Original [{sampleRow},{sampleCol}]: {originalValue}");

            var transposed = mem.TransposeMatrix(matrix, threads);

            int newValue = transposed[sampleCol, sampleRow];
            Console.WriteLine($"Transposed [{sampleCol},{sampleRow}]: {newValue}");


            //--- IO BOUND ---
            Console.WriteLine("\n--- I/O: Word Count ---");
            string testDir = "TestData";

            Console.WriteLine("Generating files...");
            TextDirectoryGenerator.GenerateFiles(testDir, 100);

            var io = new IoBoundTasks();
            long wordCount = io.CountWords(testDir, threads);

            Console.WriteLine($"Total words found: {wordCount}");
        }
    }
}
