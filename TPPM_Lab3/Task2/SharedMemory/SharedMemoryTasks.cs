using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.SharedMemory
{
    public class SharedMemoryTasks : IDisposable
    {
        private EventWaitHandle _dataReadyEvent;
        private EventWaitHandle _dataProcessedEvent;
        private Random _random;
        private SharedMemoryAccessor _accessor;

        public string Name { get; }

        public SharedMemoryTasks(string name)
        {
            Name = name;

            _random = new Random();
            _accessor = new SharedMemoryAccessor(Name);
            _dataReadyEvent = new EventWaitHandle(false, EventResetMode.AutoReset, $"{Name}_DataReady");
            _dataProcessedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, $"{Name}_DataProcessed");
        }

        public async Task MainTask(bool verbose = false)
        {
            int value = _random.Next(int.MinValue, int.MaxValue);

            if(verbose) await Console.Out.WriteLineAsync($"Sending value: {value}");

            _accessor.Write(value);

            _dataReadyEvent.Set();

            _dataProcessedEvent.WaitOne();

            value = _accessor.Read();

            if (verbose) await Console.Out.WriteLineAsync($"Recieved value: {value}");
        }

        public async Task MainTask(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int value = _random.Next(int.MinValue, int.MaxValue);

                _accessor.Write(value);

                _dataReadyEvent.Set();

                _dataProcessedEvent.WaitOne();

                value = _accessor.Read();
            }
        }

        public async Task Subtask()
        {
            _dataReadyEvent.WaitOne();

            int value = _accessor.Read();

            await File.AppendAllTextAsync("shared_mem_log_csharp.txt", $"Sub-thread int recieved back:{value}; Writing the negative");

            _accessor.Write(value * -1);

            _dataProcessedEvent.Set();
        }

        public void Dispose()
        {
            _dataReadyEvent?.Dispose();
            _dataProcessedEvent?.Dispose();
            _accessor?.Dispose();
        }
    }
}
