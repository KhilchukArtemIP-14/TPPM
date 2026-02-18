using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.MemoryBound
{
    public class MemoryBoundTasks
    {
        public int[,] TransposeMatrix(int[,] matrix, int threadCount)
        {
            int size = matrix.GetLength(0);

            int[,] result = new int[size, size];

            var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

            int chunkHeight = size / threadCount;
            int remainder = size % threadCount;

            Parallel.For(0, threadCount, options, (threadNum) =>
            {
                int startRow = threadNum * chunkHeight + (threadNum < remainder ? threadNum : remainder);

                int endRow = startRow + chunkHeight + (threadNum < remainder ? 1 : 0);

                for (int i = startRow; i < endRow; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        result[j, i] = matrix[i, j];
                    }
                }
            });

            return result;
        }
    }
}
