using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM_Lab2.Task1
{
    public class MatrixMultiplier
    {
        public static double[,] MultiplyMapReduce(double[,] A, double[,] B, int size, int maxThreads)
        {
            double[,] C = new double[size, size];

            var rowResults = Enumerable.Range(0, size)
                .AsParallel()
                .WithDegreeOfParallelism(maxThreads)
                .Select(i =>
                {
                    double[] rowResult = new double[size];

                    for (int k = 0; k < size; k++)
                    {
                        double a_ik = A[i, k];
                        for (int j = 0; j < size; j++)
                        {
                            rowResult[j] += a_ik * B[k, j];
                        }
                    }
                    return new { RowIndex = i, RowData = rowResult };
                });

            foreach (var result in rowResults)
            {
                for (int j = 0; j < size; j++)
                {
                    C[result.RowIndex, j] = result.RowData[j];
                }
            }

            return C;
        }


        public static double[,] MultiplyWorkerPool(double[,] A, double[,] B, int size, int maxThreads)
        {
            double[,] C = new double[size, size];
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            int chunkSize = size / maxThreads;
            int remainder = size % maxThreads;

            Parallel.For(0, maxThreads, options, threadNum =>
            {
                int startRow = threadNum * chunkSize + Math.Min(remainder, threadNum);
                int endRow = startRow + chunkSize + (threadNum < remainder ? 1 : 0);

                for (int i = startRow; i < endRow; i++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        double a_ik = A[i, k];
                        for (int j = 0; j < size; j++)
                        {
                            C[i, j] += a_ik * B[k, j];
                        }
                    }
                }
            });

            return C;
        }

        public static async Task<double[,]> MultiplyForkJoinAsync(double[,] A, double[,] B, int size, int threshold = 64)
        {
            double[,] C = new double[size, size];

            await MultiplyForkJoinRecursiveAsync(A, B, C, size, 0, size, threshold);

            return C;
        }

        private static async Task MultiplyForkJoinRecursiveAsync(
            double[,] A, double[,] B, double[,] C, int size, int startRow, int endRow, int threshold)
        {
            if (endRow - startRow <= threshold)
            {
                for (int i = startRow; i < endRow; i++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        double a_ik = A[i, k];
                        for (int j = 0; j < size; j++)
                        {
                            C[i, j] += a_ik * B[k, j];
                        }
                    }
                }
                return;
            }

            int mid = startRow + (endRow - startRow) / 2;

            var task1 = Task.Run(() => MultiplyForkJoinRecursiveAsync(A, B, C, size, startRow, mid, threshold));
            var task2 = Task.Run(() => MultiplyForkJoinRecursiveAsync(A, B, C, size, mid, endRow, threshold));

            await Task.WhenAll(task1, task2);
        }
    }
}
