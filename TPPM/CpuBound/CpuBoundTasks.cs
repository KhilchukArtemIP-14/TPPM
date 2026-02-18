using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPPM.Utils;

namespace TPPM.CpuBound
{
    public class CpuBoundTasks
    {
        public double CalculatePiMonteCarlo(int iterations, int threadCount)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

            int globalCount = 0;
            int baseIterations = iterations / threadCount;
            int remainder = iterations % threadCount;

            Parallel.For(0, threadCount, options,
                () => 0,
                (i, _, localCount) =>
                {
                    int iterationsCount = baseIterations + (remainder > i? 1 : 0);

                    var rnd = ThreadSafeRandom.CurrentThreadRandom;

                    for (int j =0; j < iterationsCount; j++)
                    {
                        double x = rnd.NextDouble();
                        double y = rnd.NextDouble();
                        if (x * x + y * y <= 1.0) localCount++;
                    }

                    return localCount;
                },
                (localCount) =>
                {
                    Interlocked.Add(ref globalCount, localCount);
                }
            );

            return (double)globalCount / iterations * 4.0;
        }

        public List<int> FindPrimesInRange(int start, int end, int threadCount)
        {
            var primes = new ConcurrentBag<int>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

            int stretchSize = (end - start) / threadCount;
            int remainder = (end - start) % threadCount;

            Parallel.For(0, threadCount, options, (threadNum) =>
            {
                int searchStart = threadNum * stretchSize + (remainder > threadNum ? threadNum : remainder);
                int searchEnd = searchStart + stretchSize  + (remainder > threadNum? 1 : 0);

                for(int i = searchStart; i < searchEnd; i++)
                {
                    if (IsPrime(i))
                    {
                        primes.Add(i);
                    }
                }
            });

            return new List<int>(primes);
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        public List<long> FactorizeNumber(long number, int threadCount)
        {
            var factors = new ConcurrentBag<long>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

            long limit = (long)Math.Sqrt(number);

            long range = limit - 1;

            if (range <= 0)
            {
                factors.Add(1);
                factors.Add(number);
                return new List<long>(factors);
            }

            long chunk = range / threadCount;
            long remainder = range % threadCount;

            Parallel.For(0, threadCount, options, (threadNum) =>
            {
                long startOffset = threadNum * chunk + (threadNum < remainder ? threadNum : remainder);

                long myChunkSize = chunk + (threadNum < remainder ? 1 : 0);

                long searchStart = 2 + startOffset;
                long searchEnd = searchStart + myChunkSize;

                for (long i = searchStart; i < searchEnd; i++)
                {
                    if (number % i == 0)
                    {
                        factors.Add(i);

                        if (i != number / i)
                        {
                            factors.Add(number / i);
                        }
                    }
                }
            });

            factors.Add(1);
            factors.Add(number);

            return factors.ToList();
        }
    }
}
