using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.Socket
{
    public class SocketAccessor : IDisposable
    {
        private readonly TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        public SocketAccessor(int port)
        {
            _listener = new TcpListener(IPAddress.Loopback, port);
            _listener.Start();
        }

        public void WaitForConnection()
        {
            if (_client == null)
            {
                _client = _listener.AcceptTcpClient();

                _stream = _client.GetStream();
                _reader = new BinaryReader(_stream);
                _writer = new BinaryWriter(_stream);
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
            _stream?.Dispose();
            _client?.Dispose();
            _listener?.Stop();
        }
    }
}
