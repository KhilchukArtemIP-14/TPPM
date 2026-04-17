using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.SharedMemory
{
    [MemoryDiagnoser]
    public class SharedMemoryBenchmarks
    {
        private SharedMemoryTasks _sharedMemoryTasks;
        private Process _pythonProcess;

        [GlobalSetup]
        public void Setup()
        {
            _sharedMemoryTasks = new SharedMemoryTasks("MTPP_Mem");

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            while (currentDir != null && !Directory.Exists(Path.Combine(currentDir, "SharedMemory")))
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            if (currentDir == null)
            {
                throw new DirectoryNotFoundException("Не вдалося знайти папку SharedMemory!");
            }

            string pythonScriptPath = Path.Combine(currentDir, "SharedMemory", "python", "SharedMemorySubthread.py");

            _pythonProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            });

            Thread.Sleep(2000);

            if (_pythonProcess.HasExited)
            {
                string error = _pythonProcess.StandardError.ReadToEnd();
                throw new Exception($"Python script crashed immediately! Error: {error}");
            }
        }

        [Benchmark]
        public async Task SharedMemoryTest()
        {
            await _sharedMemoryTasks.MainTask();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _sharedMemoryTasks?.Dispose();

            if (_pythonProcess != null && !_pythonProcess.HasExited)
            {
                _pythonProcess.Kill();
                _pythonProcess.Dispose();
            }
        }
    }
}
