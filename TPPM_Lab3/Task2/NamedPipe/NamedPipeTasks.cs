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

        public async Task MainTask()
        {
            int value = _random.Next(int.MinValue, int.MaxValue);

            // 1. Відправляємо в трубу
            _accessor.Write(value);

            // 2. Читаємо з труби. Потік сам почекає, поки Python не відповість!
            value = _accessor.Read();
        }

        public async Task Subtask()
        {
            // Створюємо КЛІЄНТА, який підключається до труби (на локальній машині ".")
            using var clientStream = new NamedPipeClientStream(".", Name, PipeDirection.InOut, PipeOptions.Asynchronous);

            // Чекаємо, поки сервер (MainTask) створить трубу і буде готовий
            await clientStream.ConnectAsync();

            using var reader = new BinaryReader(clientStream);
            using var writer = new BinaryWriter(clientStream);

            // Читаємо число від MainTask
            int value = reader.ReadInt32();

            // ЗАКОМЕНТОВАНО ДЛЯ БЕНЧМАРКУ:
            // await System.IO.File.AppendAllTextAsync("named_pipe_log_csharp.txt", $"Sub-thread int recieved back:{value}; Writing the negative\n");

            // Відправляємо число помножене на -1
            writer.Write(value * -1);
            writer.Flush(); // Обов'язково виштовхуємо дані
        }

        public void Dispose()
        {
            _accessor?.Dispose();
        }
    }
}
