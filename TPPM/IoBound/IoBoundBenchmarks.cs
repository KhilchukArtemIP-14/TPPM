using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM.Utils;

namespace TPPM.IoBound
{
    public class IoBoundBenchmarks
    {
        private IoBoundTasks _ioTasks;
        private string _testDir;

        [Params(1, 2, 4, 8, 12, 16, 20, 24, 36)]
        public int ThreadCount;

        [GlobalSetup]
        public void Setup()
        {
            _ioTasks = new IoBoundTasks();
            _testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Benchmark_IO_Data");

            TextDirectoryGenerator.GenerateFiles(_testDir, 1000);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Benchmark]
        public long CountWords()
        {
            return _ioTasks.CountWords(_testDir, ThreadCount);
        }
    }
}
