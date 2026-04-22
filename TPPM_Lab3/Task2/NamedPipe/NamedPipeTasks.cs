using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.NamedPipe
{
    public class NamedPipeTasks : IDisposable
    {
        private Random _random;
        private NamedPipeAccessor _accessor;

        public string Name { get; }

        public NamedPipeTasks(string name)
        {
            Name = name;
            _random = new Random();
            _accessor = new NamedPipeAccessor(Name);
        }

        public void WaitForClient()
        {
            _accessor.WaitForConnection();
        }

        public async Task MainTask(bool verbose = false)
        {
            int value = _random.Next(int.MinValue, int.MaxValue);

            if (verbose) await Console.Out.WriteLineAsync($"Sending value: {value}");

            _accessor.Write(value);

            value = _accessor.Read();

            if (verbose) await Console.Out.WriteLineAsync($"Recieved value: {value}");
        }
        public async Task MainTask(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int value = _random.Next(int.MinValue, int.MaxValue);

                _accessor.Write(value);

                value = _accessor.Read();
            }
        }
        public async Task Subtask()
        {
            using var clientStream = new NamedPipeClientStream(".", Name, PipeDirection.InOut, PipeOptions.Asynchronous);

            await clientStream.ConnectAsync();

            using var reader = new BinaryReader(clientStream);
            using var writer = new BinaryWriter(clientStream);

            int value = reader.ReadInt32();

            writer.Write(value * -1);
            writer.Flush(); 
        }

        public void Dispose()
        {
            _accessor?.Dispose();
        }
    }
}
