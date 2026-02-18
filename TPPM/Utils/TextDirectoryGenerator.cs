using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.Utils
{
    public static class TextDirectoryGenerator
    {
        private static readonly string[] Vocab = { "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "code", "laba", "tppm" };

        public static void GenerateFiles(string rootPath, int fileCount, int maxDepth = 8)
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
            Directory.CreateDirectory(rootPath);

            Random rand = new Random();

            for (int i = 0; i < fileCount; i++)
            {
                string currentPath = rootPath;
                int depth = rand.Next(0, maxDepth + 1);

                for (int d = 0; d < depth; d++)
                {
                    currentPath = Path.Combine(currentPath, $"subdir_{rand.Next(1, 10)}");
                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                }

                string filePath = Path.Combine(currentPath, $"file_{i}.txt");
                string content = GenerateRandomText(rand.Next(50, 500));
                File.WriteAllText(filePath, content);
            }
        }

        private static string GenerateRandomText(int wordCount)
        {
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < wordCount; i++)
            {
                sb.Append(Vocab[rand.Next(Vocab.Length)]).Append(" ");
            }
            return sb.ToString();
        }
    }
}
