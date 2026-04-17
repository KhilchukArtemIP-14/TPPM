using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab2.Task1
{
    public static class ParallelQuickSelect
    {
        public static int QuickSelectMapReduce(int[] arr, int k, int maxThreads)
        {
            if (arr.Length == 1) return arr[0];
            int pivot = arr[arr.Length / 2];

            var counts = arr.AsParallel().WithDegreeOfParallelism(maxThreads).Aggregate(
                () => new { Less = 0, Equal = 0 },
                (loc, val) => val < pivot ? new { Less = loc.Less + 1, Equal = loc.Equal } :
                              val == pivot ? new { Less = loc.Less, Equal = loc.Equal + 1 } : loc,
                (glob, loc) => new { Less = glob.Less + loc.Less, Equal = glob.Equal + loc.Equal },
                f => f
            );

            if (k < counts.Less)
                return QuickSelectMapReduce(arr.AsParallel().Where(x => x < pivot).ToArray(), k, maxThreads);
            if (k < counts.Less + counts.Equal)
                return pivot;

            return QuickSelectMapReduce(arr.AsParallel().Where(x => x > pivot).ToArray(), k - counts.Less - counts.Equal, maxThreads);
        }

        public static int QuickSelectWorkerPool(int[] arr, int k, int maxThreads)
        {
            if (arr.Length == 1) return arr[0];
            int pivot = arr[arr.Length / 2];

            int less = 0, equal = 0;
            object syncLock = new object();
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            int chunkSize = arr.Length / maxThreads;
            if (chunkSize == 0) chunkSize = 1;
            int remainder = arr.Length % maxThreads;
            int threads = Math.Min(arr.Length, maxThreads);

            Parallel.For(0, threads, options, threadNum =>
            {
                int start = threadNum * chunkSize + Math.Min(remainder, threadNum);
                int end = start + chunkSize + (threadNum < remainder ? 1 : 0);
                int localLess = 0, localEqual = 0;

                for (int i = start; i < end; i++)
                {
                    if (arr[i] < pivot) localLess++;
                    else if (arr[i] == pivot) localEqual++;
                }

                lock (syncLock) { less += localLess; equal += localEqual; }
            });

            if (k < less)
                return QuickSelectWorkerPool(arr.AsParallel().Where(x => x < pivot).ToArray(), k, maxThreads);
            if (k < less + equal) return pivot;

            return QuickSelectWorkerPool(arr.AsParallel().Where(x => x > pivot).ToArray(), k - less - equal, maxThreads);
        }

        public static async Task<int> QuickSelectForkJoinAsync(int[] arr, int k, int threshold)
        {
            if (arr.Length == 1) return arr[0];
            int pivot = arr[arr.Length / 2];

            var counts = await CountPivotForkJoinAsync(arr, pivot, 0, arr.Length, threshold);

            if (k < counts.Less)
                return await QuickSelectForkJoinAsync(arr.Where(x => x < pivot).ToArray(), k, threshold);
            if (k < counts.Less + counts.Equal)
                return pivot;

            return await QuickSelectForkJoinAsync(arr.Where(x => x > pivot).ToArray(), k - counts.Less - counts.Equal, threshold);
        }

        private static async Task<(int Less, int Equal)> CountPivotForkJoinAsync(int[] arr, int pivot, int start, int end, int threshold)
        {
            if (end - start <= threshold)
            {
                int localLess = 0, localEqual = 0;
                for (int i = start; i < end; i++)
                {
                    if (arr[i] < pivot) localLess++;
                    else if (arr[i] == pivot) localEqual++;
                }
                return (localLess, localEqual);
            }

            int mid = start + (end - start) / 2;
            var left = Task.Run(() => CountPivotForkJoinAsync(arr, pivot, start, mid, threshold));
            var right = Task.Run(() => CountPivotForkJoinAsync(arr, pivot, mid, end, threshold));

            var res = await Task.WhenAll(left, right);
            return (res[0].Less + res[1].Less, res[0].Equal + res[1].Equal);
        }
    }
}
