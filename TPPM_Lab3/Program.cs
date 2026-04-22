using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TPPM_Lab3.Task1.TransferStrategies;
using TPPM_Lab3.Task1;
using TPPM_Lab3.Task2.NamedPipe;
using TPPM_Lab3.Task2.SharedMemory;
using TPPM_Lab3.Task2.Socket;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using TPPM_Lab3.Utils;

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
            
            var config = DefaultConfig.Instance
                .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
                .WithOptions(ConfigOptions.DisableLogFile);

            Console.WriteLine("Would you like to run SharedMemory benchmarks? y/n");
            key = Console.ReadLine();
            while (key != "y" && key != "n")
            {
                Console.WriteLine("Please, type y or n");
                key = Console.ReadLine();
            }
            if (key == "y")
            {
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
                BenchmarkRunner.Run<SocketBenchmarks>(config, args);
            }

            Console.WriteLine("=== Shared Memory (C# <-> C#) ===");

            using (var smTasks = new SharedMemoryTasks("MTPP_Mem"))
            {
                string smPythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Task2", "SharedMemory", "python", "SharedMemorySubthread.py");
                using (var smPythonProcess = new PythonSubprocessManager(smPythonScriptPath))
                {
                    //var subTask = Task.Run(() => smTasks.Subtask());
                    var mainTask = Task.Run(() => smTasks.MainTask(true));

                    await Task.WhenAll(mainTask);
                }
            }

            Console.WriteLine("=== Named Pipe (C# <-> C#) ===");

            using (var smTasks = new NamedPipeTasks("MTPP_Pipe"))
            {
                string npPythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Task2", "NamedPipe", "python", "NamedPipeSubthread.py");
                using (var npPythonProcess = new PythonSubprocessManager(npPythonScriptPath))
                {
                    smTasks.WaitForClient();

                    //var subTask = Task.Run(() => smTasks.Subtask());
                    var mainTask = Task.Run(() => smTasks.MainTask(true));

                    await Task.WhenAll(mainTask);
                }
            }

            Console.WriteLine("=== Sockets (C# <-> C#) ===");

            using (var smTasks = new SocketTasks())
            {
                string socketPythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Task2", "Socket", "python", "SocketSubthread.py");
                using(var socketPythonProcess = new PythonSubprocessManager(socketPythonScriptPath))
                {
                    smTasks.WaitForClient();
                    //var subTask = Task.Run(() => smTasks.Subtask());
                    var mainTask = Task.Run(() => smTasks.MainTask(true));

                    await Task.WhenAll(mainTask);
                }
            }
        }
    }
}
