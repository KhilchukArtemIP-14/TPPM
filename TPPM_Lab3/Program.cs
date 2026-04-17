using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TPPM_Lab3.Task1.TransferStrategies;
using TPPM_Lab3.Task1;
using TPPM_Lab3.Task2.NamedPipe;
using TPPM_Lab3.Task2.SharedMemory;
using TPPM_Lab3.Task2.Socket;

namespace TPPM_Lab3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var simulationTime = TimeSpan.FromSeconds(10);

            Console.WriteLine("===Simulating naive bank transfer logic===");
            var badBank = new Bank(100, new TransferRaceCondition());

            decimal badInitial = badBank.TotalBalance;
            Console.WriteLine($"Initial balance: {badInitial}");

            await badBank.RunSimulationAsync(simulationTime);

            decimal badFinal = badBank.TotalBalance;
            Console.WriteLine($"Final balance: {badFinal}");
            Console.WriteLine($"Balance diff: {badFinal - badInitial}\n");


            Console.WriteLine("Would you like to run Deadlock simulation? y/n");
            var key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }

            if (key == "y")
            {
                Console.WriteLine("===Simulating deadlock-prone bank transfer logic===");
                var deadlockBank = new Bank(100, new TransferDeadlock());

                decimal deadlockInitial = deadlockBank.TotalBalance;
                Console.WriteLine($"Initial balance: {deadlockInitial}");

                await deadlockBank.RunSimulationAsync(simulationTime);

                decimal deadlockFinal = deadlockBank.TotalBalance;
                Console.WriteLine($"Final balance: {deadlockFinal}");
                Console.WriteLine($"Balance diff: {deadlockFinal - deadlockInitial}\n");
            }

            Console.WriteLine("===Simulating safe bank transfer logic===");
            var goodBank = new Bank(100, new TransferSafe());

            decimal goodInitial = goodBank.TotalBalance;
            Console.WriteLine($"Initial balance: {goodInitial}");

            await goodBank.RunSimulationAsync(simulationTime);

            decimal goodFinal = goodBank.TotalBalance;
            Console.WriteLine($"Final balance: {goodFinal}");
            Console.WriteLine($"Balance diff: {goodFinal - goodInitial}\n");



            Console.WriteLine("Would you like to run SharedMemory benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<SharedMemoryBenchmarks>(config, args);
            }

            Console.WriteLine("Would you like to run NamedPipes benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<NamedPipeBenchmarks>(config, args);
            }


            Console.WriteLine("Would you like to run Socket benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
                var config = DefaultConfig.Instance;
                BenchmarkRunner.Run<SocketBenchmarks>(config, args);
            }

            Console.WriteLine("=== Тестування Shared Memory (C# <-> C#) ===");

            using (var smTasks = new SharedMemoryTasks("LocalTestMem"))
            {
                var subTask = Task.Run(() => smTasks.Subtask());
                var mainTask = Task.Run(() => smTasks.MainTask());

                await Task.WhenAll(mainTask, subTask);
            }

            Console.WriteLine("=== Обмін успішно завершено! ===");
        }
    }
}
