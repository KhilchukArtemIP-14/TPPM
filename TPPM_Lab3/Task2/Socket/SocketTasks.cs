using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.Socket
{
    public class SocketTasks : IDisposable
    {
        private Random _random;
        private SocketAccessor _accessor;
        private int _port;

        public SocketTasks(int port = 5050)
        {
            _port = port;
            _random = new Random();
            _accessor = new SocketAccessor(_port);
        }

        public void WaitForClient()
        {
            _accessor.WaitForConnection();
        }

        public async Task MainTask(bool verbose = false)
        {
            int value = _random.Next(int.MinValue, int.MaxValue);

            if(verbose) await Console.Out.WriteLineAsync($"Sending value: {value}");

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
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", _port);

            using var stream = client.GetStream();
            using var reader = new BinaryReader(stream);
            using var writer = new BinaryWriter(stream);

            int value = reader.ReadInt32();

            await File.AppendAllTextAsync("socket_log_csharp.txt", $"Sub-thread received: {value}\n");

            writer.Write(value * -1);
            writer.Flush();
        }

        public void Dispose()
        {
            _accessor?.Dispose();
        }
    }
}
