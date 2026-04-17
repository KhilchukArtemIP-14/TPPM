using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab2.Models;
using TPPM_Lab2.Utils;

namespace TPPM_Lab2.Task1
{
    public enum Task1Patterns
    {
        MapReduce,
        WorkerPool,
        ForkJoin
    }

    [MemoryDiagnoser]
    public class Task1Benchmarks
    {
        [ParamsAllValues]
        public Task1Patterns Pattern { get; set; }

        private int _threads;

        private int[] _numbers;

        private List<string> _htmlDocuments;

        private double[,] _matrixA;
        private double[,] _matrixB;
        private int _matrixSize = 1000; 

        [GlobalSetup]
        public void Setup()
        {
            _threads = Environment.ProcessorCount;
            var rnd = new Random();

            _numbers = new int[1_000_000];
            for (int i = 0; i < _numbers.Length; i++)
            {
                _numbers[i] = rnd.Next(0, 100_000);
            }

            _htmlDocuments = HtmlGenerator.GenerateDocumentsAndSave(1000, "HtmlConcurrencyFiles", 6, 5);

            _matrixA = new double[_matrixSize, _matrixSize];
            _matrixB = new double[_matrixSize, _matrixSize];
            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    _matrixA[i, j] = rnd.NextDouble();
                    _matrixB[i, j] = rnd.NextDouble();
                }
            }
        }

        [Benchmark]
        public async Task<StatsResult> ArrayBenchmark()
        {
            return Pattern switch
            {
                Task1Patterns.MapReduce => ArrayStatistics.GetStatsMapReduce(_numbers, _threads),
                Task1Patterns.WorkerPool => ArrayStatistics.GetStatsWorkerPool(_numbers, _threads),
                Task1Patterns.ForkJoin => await ArrayStatistics.GetStatsForkJoinAsync(_numbers, 10000),
                _ => throw new Exception()
            };
        }

        [Benchmark]
        public async Task<Dictionary<string, int>> HtmlTagsBenchmark()
        {
            return Pattern switch
            {
                Task1Patterns.MapReduce => HtmlTagCounter.CountTagsMapReduce(_htmlDocuments, _threads),
                Task1Patterns.WorkerPool => HtmlTagCounter.CountTagsWorkerPool(_htmlDocuments, _threads),
                Task1Patterns.ForkJoin => await HtmlTagCounter.CountTagsForkJoinAsync(_htmlDocuments, 50),
                _ => throw new Exception()
            };
        }

        [Benchmark]
        public async Task<double[,]> MatrixBenchmark()
        {
            return Pattern switch
            {
                Task1Patterns.MapReduce => MatrixMultiplier.MultiplyMapReduce(_matrixA, _matrixB, _matrixSize, _threads),
                Task1Patterns.WorkerPool => MatrixMultiplier.MultiplyWorkerPool(_matrixA, _matrixB, _matrixSize, _threads),
                Task1Patterns.ForkJoin => await MatrixMultiplier.MultiplyForkJoinAsync(_matrixA, _matrixB, _matrixSize, 64),
                _ => throw new Exception()
            };
        }
    }
}
