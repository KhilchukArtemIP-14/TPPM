using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.NamedPipe
{
    [MemoryDiagnoser]
    public class NamedPipeBenchmarks
    {
        private NamedPipeTasks _tasks;
        private Process _pythonProcess;

        [GlobalSetup]
        public void Setup()
        {
            _tasks = new NamedPipeTasks("MTPP_Pipe");

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            while (currentDir != null && !Directory.Exists(Path.Combine(currentDir, "NamedPipe")))
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            if (currentDir == null)
            {
                throw new DirectoryNotFoundException("Не вдалося знайти папку NamedPipes!");
            }

            // Шлях до нового скрипта у папці NamedPipes/python
            string pythonScriptPath = Path.Combine(currentDir, "NamedPipe", "python", "NamedPipeSubthread.py");

            _pythonProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            });

            Thread.Sleep(2000); // Даємо Python час на запуск

            if (_pythonProcess.HasExited)
            {
                string error = _pythonProcess.StandardError.ReadToEnd();
                throw new Exception($"Python crashed: {error}");
            }

            // ЧЕКАЄМО, ПОКИ PYTHON ПІДКЛЮЧИТЬСЯ ДО ТРУБИ
            _tasks.WaitForClient();
        }

        [Benchmark]
        public async Task NamedPipeTest()
        {
            await _tasks.MainTask();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _tasks?.Dispose();

            if (_pythonProcess != null && !_pythonProcess.HasExited)
            {
                _pythonProcess.Kill();
                _pythonProcess.Dispose();
            }
        }
    }
}
