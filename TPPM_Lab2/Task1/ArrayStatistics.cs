using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM_Lab2.Models;

namespace TPPM_Lab2.Task1
{
    public class ArrayStatistics
    {
        public static StatsResult GetStatsMapReduce(int[] numbers, int maxThreads)
        {
            var parallelNumbers = numbers.AsParallel().WithDegreeOfParallelism(maxThreads);

            var stats = parallelNumbers.Aggregate(
                () => new { Min = int.MaxValue, Max = int.MinValue, Sum = 0L, Count = 0 },
                (local, val) => new {
                    Min = Math.Min(local.Min, val),
                    Max = Math.Max(local.Max, val),
                    Sum = local.Sum + val,
                    Count = local.Count + 1
                },
                (global, local) => new {
                    Min = Math.Min(global.Min, local.Min),
                    Max = Math.Max(global.Max, local.Max),
                    Sum = global.Sum + local.Sum,
                    Count = global.Count + local.Count
                },
                final => final
            );

            int rightMid = ParallelQuickSelect.QuickSelectMapReduce(numbers, numbers.Length / 2, maxThreads);
            double median = rightMid;
            if (numbers.Length % 2 == 0)
            {
                int leftMid = ParallelQuickSelect.QuickSelectMapReduce(numbers, numbers.Length / 2 - 1, maxThreads);
                median = (leftMid + rightMid) / 2.0;
            }

            return new StatsResult() { Min = stats.Min, Max = stats.Max, Average = stats.Sum / (double)stats.Count, Median = median };
        }
        public static StatsResult GetStatsWorkerPool(int[] numbers, int maxThreads)
        {
            int globalMin = int.MaxValue, globalMax = int.MinValue;
            long globalSum = 0;
            object syncLock = new object();

            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };
            var chunkSize = numbers.Length / maxThreads;
            var remainder = numbers.Length % maxThreads;

            Parallel.For(0, maxThreads, options, (threadNum) =>
            {
                var start = threadNum * chunkSize + Math.Min(remainder, threadNum);
                var end = start + chunkSize + (threadNum < remainder ? 1 : 0);
                int localMin = int.MaxValue, localMax = int.MinValue;
                long localSum = 0;

                for (int i = start; i < end; i++)
                {
                    if (numbers[i] < localMin) localMin = numbers[i];
                    if (numbers[i] > localMax) localMax = numbers[i];
                    localSum += numbers[i];
                }

                lock (syncLock)
                {
                    if (localMin < globalMin) globalMin = localMin;
                    if (localMax > globalMax) globalMax = localMax;
                    globalSum += localSum;
                }
            });

            int rightMid = ParallelQuickSelect.QuickSelectWorkerPool(numbers, numbers.Length / 2, maxThreads);
            double median = rightMid;
            if (numbers.Length % 2 == 0)
            {
                int leftMid = ParallelQuickSelect.QuickSelectWorkerPool(numbers, numbers.Length / 2 - 1, maxThreads);
                median = (leftMid + rightMid) / 2.0;
            }

            return new StatsResult() { Min = globalMin, Max = globalMax, Average = globalSum / (double)numbers.Length, Median = median };
        }

        public static async Task<StatsResult> GetStatsForkJoinAsync(int[] numbers, int threshold = 10000)
        {
            var result = await GetStatsForkJoinRecursiveAsync(numbers, 0, numbers.Length, threshold);

            int rightMid = await ParallelQuickSelect.QuickSelectForkJoinAsync(numbers, numbers.Length / 2, threshold);
            double median = rightMid;
            if (numbers.Length % 2 == 0)
            {
                int leftMid = await ParallelQuickSelect.QuickSelectForkJoinAsync(numbers, numbers.Length / 2 - 1, threshold);
                median = (leftMid + rightMid) / 2.0;
            }

            return new StatsResult() { Min = result.Min, Max = result.Max, Average = result.Sum / (double)result.Count, Median = median };
        }

        private static async Task<LocalStats> GetStatsForkJoinRecursiveAsync(int[] numbers, int start, int end, int threshold)
        {
            if (end - start <= threshold)
            {
                int localMin = int.MaxValue, localMax = int.MinValue;
                long localSum = 0;
                for (int i = start; i < end; i++)
                {
                    if (numbers[i] < localMin) localMin = numbers[i];
                    if (numbers[i] > localMax) localMax = numbers[i];
                    localSum += numbers[i];
                }
                return new LocalStats() { Min = localMin, Max = localMax, Sum = localSum, Count = end - start };
            }

            int mid = start + (end - start) / 2;
            var leftTask = Task.Run(() => GetStatsForkJoinRecursiveAsync(numbers, start, mid, threshold));
            var rightTask = Task.Run(() => GetStatsForkJoinRecursiveAsync(numbers, mid, end, threshold));

            var res = await Task.WhenAll(leftTask, rightTask);
            return new LocalStats()
            {
                Min = Math.Min(res[0].Min, res[1].Min),
                Max = Math.Max(res[0].Max, res[1].Max),
                Sum = res[0].Sum + res[1].Sum,
                Count = res[0].Count + res[1].Count
            };
        }
    }
}
