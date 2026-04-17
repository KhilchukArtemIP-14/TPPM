using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.NamedPipe
{
    public class NamedPipeAccessor : IDisposable
    {
        public string Name { get; }
        private readonly NamedPipeServerStream _serverStream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public NamedPipeAccessor(string name)
        {
            Name = name;

            _serverStream = new NamedPipeServerStream(
                Name,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            _reader = new BinaryReader(_serverStream);
            _writer = new BinaryWriter(_serverStream);
        }

        public void WaitForConnection()
        {
            if (!_serverStream.IsConnected)
            {
                Console.WriteLine("Waiting for client connection...");
                _serverStream.WaitForConnection();
                Console.WriteLine("Connected!");
            }
        }

        public bool Write(int value)
        {
            _writer.Write(value);
            _writer.Flush();
            return true;
        }

        public int Read()
        {
            return _reader.ReadInt32();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _serverStream?.Dispose();
        }
    }
}
