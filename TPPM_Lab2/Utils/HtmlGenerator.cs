using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TPPM_Lab2.Utils
{
    public static class HtmlGenerator
    {
        private static readonly string[] Tags =
        {
            "div", "p", "a", "span", "h1", "h2", "h3",
            "ul", "li", "article", "section", "main", "header", "footer", "strong", "em"
        };

        private static readonly string[] Words =
        {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
            "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt"
        };

        public static List<string> GenerateDocumentsAndSave(int count, string outputDirectory="HtmlConcurrencyFiles", int maxDepth = 5, int maxSiblings = 4)
        {
            var filePaths = new List<string>(count);
            var rnd = new Random();

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            for (int i = 0; i < count; i++)
            {
                var sb = new StringBuilder();

                GenerateNode(sb, rnd, 0, maxDepth, maxSiblings);

                string filePath = Path.Combine(outputDirectory, $"document_{i + 1}.html");
                File.WriteAllText(filePath, sb.ToString());

                filePaths.Add(filePath);
            }

            return filePaths;
        }

        private static void GenerateNode(StringBuilder sb, Random rnd, int currentDepth, int maxDepth, int maxSiblings)
        {
            if (currentDepth >= maxDepth)
            {
                sb.Append(GenerateRandomText(rnd));
                return;
            }

            int numSiblings = rnd.Next(1, maxSiblings + 1);

            for (int i = 0; i < numSiblings; i++)
            {
                string tag = Tags[rnd.Next(Tags.Length)];

                sb.Append($"<{tag}>");

                if (rnd.NextDouble() < 0.3)
                {
                    sb.Append(GenerateRandomText(rnd));
                }
                else
                {
                    GenerateNode(sb, rnd, currentDepth + 1, maxDepth, maxSiblings);
                }

                sb.Append($"</{tag}>\n");
            }
        }

        private static string GenerateRandomText(Random rnd)
        {
            int wordCount = rnd.Next(2, 8);
            var sb = new StringBuilder();
            for (int i = 0; i < wordCount; i++)
            {
                sb.Append(Words[rnd.Next(Words.Length)]).Append(" ");
            }
            return sb.ToString();
        }
    }
}