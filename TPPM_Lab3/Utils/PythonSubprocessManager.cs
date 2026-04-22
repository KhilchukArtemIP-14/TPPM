using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Utils
{
    public class PythonSubprocessManager : IDisposable
    {
        private readonly Process _process;

        public PythonSubprocessManager(string path)
        {
            _process = Process.Start(new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{path}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            });

            if (_process == null)
            {
                throw new InvalidOperationException("Не вдалося запустити процес Python.");
            }

            Thread.Sleep(2000);

            if (_process.HasExited)
            {
                string error = _process.StandardError.ReadToEnd();
                throw new Exception($"Error: {error}");
            }
        }

        public void Dispose()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.Dispose();
            }
        }
    }
}
