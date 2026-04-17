using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TPPM_Lab2.Task1
{
    public class HtmlTagCounter
    {
        private static readonly Regex TagRegex = new Regex(@"<([a-z0-9]+)\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static IEnumerable<string> ExtractTags(string html) =>
            TagRegex.Matches(html).Select(m => m.Groups[1].Value.ToLower());

        public static Dictionary<string, int> CountTagsMapReduce(List<string> filePaths, int maxThreads)
        {
            return filePaths.AsParallel()
                .WithDegreeOfParallelism(maxThreads)
                .SelectMany(path => ExtractTags(File.ReadAllText(path)))
                .GroupBy(tag => tag)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public static Dictionary<string, int> CountTagsWorkerPool(List<string> filePaths, int maxThreads)
        {
            var tagCounts = new ConcurrentDictionary<string, int>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            var chunkSize = filePaths.Count / maxThreads;
            var remainder = filePaths.Count % chunkSize;

            Parallel.For(0, maxThreads, options, (threadNum) =>
            {
                var start = threadNum * chunkSize + Math.Min(remainder, threadNum);
                var end = start + chunkSize + (threadNum < remainder ? 1 : 0);

                for (int i = start; i < end; i++)
                {
                    string htmlContent = File.ReadAllText(filePaths[i]);
                    var tags = ExtractTags(htmlContent);

                    foreach (var tag in tags)
                    {
                        tagCounts.AddOrUpdate(tag, 1, (_, count) => count + 1);
                    }
                }
            });

            return tagCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static Task<Dictionary<string, int>> CountTagsForkJoinAsync(List<string> filePaths, int threshold = 50)
        {
            return CountTagsForkJoinRecursiveAsync(filePaths, 0, filePaths.Count, threshold);
        }

        private static async Task<Dictionary<string, int>> CountTagsForkJoinRecursiveAsync(
            List<string> filePaths, int start, int end, int threshold)
        {
            if (end - start <= threshold)
            {
                var localDict = new Dictionary<string, int>();
                for (int i = start; i < end; i++)
                {
                    string htmlContent = await File.ReadAllTextAsync(filePaths[i]);

                    foreach (var tag in ExtractTags(htmlContent))
                    {
                        if (!localDict.ContainsKey(tag)) localDict[tag] = 0;
                        localDict[tag]++;
                    }
                }
                return localDict;
            }

            int mid = start + (end - start) / 2;

            var leftTask = Task.Run(() => CountTagsForkJoinRecursiveAsync(filePaths, start, mid, threshold));
            var rightTask = Task.Run(() => CountTagsForkJoinRecursiveAsync(filePaths, mid, end, threshold));

            var results = await Task.WhenAll(leftTask, rightTask);

            return MergeDictionaries(results[0], results[1]);
        }

        private static Dictionary<string, int> MergeDictionaries(
            Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            var result = new Dictionary<string, int>(dict1);
            foreach (var kvp in dict2)
            {
                if (result.ContainsKey(kvp.Key))
                    result[kvp.Key] += kvp.Value;
                else
                    result[kvp.Key] = kvp.Value;
            }
            return result;
        }
    }
}