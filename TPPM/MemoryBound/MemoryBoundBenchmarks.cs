using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM.Utils;

namespace TPPM.MemoryBound
{
    [MemoryDiagnoser]
    public class MemoryBoundBenchmarks
    {
        private MemoryBoundTasks _memTasks;
        private int[,] _sourceMatrix;

        [Params(1, 2, 4, 8, 12, 16, 20, 24, 36)]
        public int ThreadCount;

        [GlobalSetup]
        public void Setup()
        {
            _memTasks = new MemoryBoundTasks();
            _sourceMatrix = MatrixGenerator.GenerateMatrix(10000);
        }

        [Benchmark]
        public int[,] TransposeMatrix()
        {
            return _memTasks.TransposeMatrix(_sourceMatrix, ThreadCount);
        }
    }
}
