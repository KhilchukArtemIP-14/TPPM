using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.IoBound
{
    public class IoBoundTasks
    {
        public long CountWords(string rootPath, int threadCount)
        {
            long totalWords = 0;

            var files = Directory.GetFiles(rootPath, "*.txt", SearchOption.AllDirectories);

            int totalFiles = files.Length;
            if (totalFiles == 0) return 0;

            var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

            int chunk = totalFiles / threadCount;
            int remainder = totalFiles % threadCount;

            Parallel.For(0, threadCount, options, (threadNum) =>
            {
                int startIndex = threadNum * chunk + (threadNum < remainder ? threadNum : remainder);
                int endIndex = startIndex + chunk + (threadNum < remainder ? 1 : 0);

                long localThreadCount = 0;

                for (int i = startIndex; i < endIndex; i++)
                {
                    string filePath = files[i];
                    try
                    {
                        string text = File.ReadAllText(filePath);

                        bool inWord = false;
                        for (int c = 0; c < text.Length; c++)
                        {
                            if (char.IsWhiteSpace(text[c]))
                            {
                                inWord = false;
                            }
                            else if (!inWord)
                            {
                                localThreadCount++;
                                inWord = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading {filePath}: {ex.Message}");
                    }
                }

                if (localThreadCount > 0)
                {
                    Interlocked.Add(ref totalWords, localThreadCount);
                }
            });

            return totalWords;
        }
    }
}
