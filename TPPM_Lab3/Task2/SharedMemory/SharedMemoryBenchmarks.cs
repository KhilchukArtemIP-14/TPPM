using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab3.Utils;

namespace TPPM_Lab3.Task2.SharedMemory
{
    [MemoryDiagnoser]
    public class SharedMemoryBenchmarks
    {
        private SharedMemoryTasks _sharedMemoryTasks;
        private PythonSubprocessManager _pythonProcess;
        public const int IterationsCount = 10_000;

        [GlobalSetup]
        public void Setup()
        {
            _sharedMemoryTasks = new SharedMemoryTasks("MTPP_Mem");

            string pythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Task2", "SharedMemory", "python", "SharedMemorySubthread.py");
            _pythonProcess = new PythonSubprocessManager(pythonScriptPath);
        }

        [Benchmark]
        public async Task SharedMemoryTest()
        {
            await _sharedMemoryTasks.MainTask(IterationsCount);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _sharedMemoryTasks?.Dispose();
            _pythonProcess?.Dispose();
        }
    }
}
