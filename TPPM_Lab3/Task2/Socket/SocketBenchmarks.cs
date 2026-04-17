using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.Socket
{
    [MemoryDiagnoser]
    public class SocketBenchmarks
    {
        private SocketTasks _tasks;
        private Process _pythonProcess;

        [GlobalSetup]
        public void Setup()
        {
            _tasks = new SocketTasks(5050);

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            while (currentDir != null && !Directory.Exists(Path.Combine(currentDir, "Socket")))
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            if (currentDir == null)
            {
                throw new DirectoryNotFoundException("Не вдалося знайти папку Sockets!");
            }

            string pythonScriptPath = Path.Combine(currentDir, "Socket", "python", "SocketSubthread.py");

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
                throw new Exception($"Python crashed: {error}");
            }

            _tasks.WaitForClient();
        }

        [Benchmark]
        public async Task SocketTest()
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
