using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merezha
{
    class Reading
    {

        public List<int[]> ReadFile(string path)
        {
            string[] lines = File.ReadAllLines(path).ToArray();
            List<int[]> arr = new List<int[]>();
            // разобрать в массив
            for (int i = 0; i < lines.Length; i++)
            {
                int[] row = lines[i].Split(new char[] { ' ', '-' }).Select(Int32.Parse).ToArray();
                arr.Add(row);
            }
            return arr;
        }


        public int MaxValue(List<int[]> arr)
        {
            int max = 1;
            for (int i = 0;i<arr.Count;i++)
                for (int j = 0; j < 2;j++)
                {
                    if (arr[i][j] > max)
                        max = arr[i][j];
                }
            return max;
        }
    }
}
