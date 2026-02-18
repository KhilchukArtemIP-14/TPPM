using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.CpuBound
{
    [MemoryDiagnoser]
    public class CpuBoundBenchmarks
    {
        private CpuBoundTasks _cpuTasks;
        private long _largeNumber;
        private int _piIterations;
        private int _primesEnd;

        [Params(1, 2, 4, 8, 12, 16, 20, 24, 36)]
        public int ThreadCount;

        [GlobalSetup]
        public void Setup()
        {
            _cpuTasks = new CpuBoundTasks();
            _piIterations = 10_000_000;
            _primesEnd = 500_000;
            _largeNumber = 489133282872437279;
        }

        [Benchmark]
        public double CalculatePi()
        {
            return _cpuTasks.CalculatePiMonteCarlo(_piIterations, ThreadCount);
        }

        [Benchmark]
        public object FindPrimes()
        {
            return _cpuTasks.FindPrimesInRange(0, _primesEnd, ThreadCount);
        }

        [Benchmark]
        public object Factorize()
        {
            return _cpuTasks.FactorizeNumber(_largeNumber, ThreadCount);
        }
    }
}
