using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab3.Task2.SharedMemory
{
    public class SharedMemoryAccessor : IDisposable
    {
        public string Name { get; }
        private readonly int _memorySize;
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;

        public SharedMemoryAccessor(string name, int memorySize = 1024)
        {
            _memorySize = memorySize;
            Name = name;

            _mmf = MemoryMappedFile.CreateOrOpen(Name, _memorySize);
            _accessor = _mmf.CreateViewAccessor(0, _memorySize);
        }

        public bool Write(int value)
        {
            _accessor.Write(0, value);

            return true;
        }

        public int Read()
        {
            return _accessor.ReadInt32(0);
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }
}
