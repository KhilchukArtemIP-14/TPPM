using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.Utils
{
    public static class MatrixGenerator
    {
        public static int[,] GenerateMatrix(int size)
        {
            var matrix = new int[size, size];
            var rand = new Random();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = rand.Next(int.MinValue, int.MaxValue);
                }
            }

            return matrix;
        }

    }
}
