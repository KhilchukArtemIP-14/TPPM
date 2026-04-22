using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab3.Utils;

namespace TPPM_Lab3.Task2.NamedPipe
{
    [MemoryDiagnoser]
    public class NamedPipeBenchmarks
    {
        private NamedPipeTasks _tasks;
        private PythonSubprocessManager _pythonProcess;
        public const int IterationsCount = 10_000;

        [GlobalSetup]
        public void Setup()
        {
            _tasks = new NamedPipeTasks("MTPP_Pipe");

            string pythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Task2" , "NamedPipe", "python", "NamedPipeSubthread.py");

            _pythonProcess = new PythonSubprocessManager(pythonScriptPath);

            _tasks.WaitForClient();
        }

        [Benchmark]
        public async Task NamedPipeTest()
        {
            await _tasks.MainTask(IterationsCount);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _tasks?.Dispose();
            _pythonProcess?.Dispose();
        }
    }
}
