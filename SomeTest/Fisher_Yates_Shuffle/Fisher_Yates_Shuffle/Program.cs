using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fisher_Yates_Shuffle
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> numList = new List<int>() { 1, 2, 3, 4, 5 };
            List<string> stringList = new List<string>() { "A", "B", "C", "D", "E" };

            numList.Shuffle();
            foreach (var tmp in numList)
            {
                Console.WriteLine(tmp);
            }

            stringList.Shuffle();
            foreach (var tmp in stringList)
            {
                Console.WriteLine(tmp);
            }
        }
    }

    /// <summary>
    /// 随机乱序数组中的元素(使用Fisher–Yates shuffle algorithm)
    /// </summary>
    static class ShuffleHelper
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random randomGenerator = new Random();
            int count = list.Count;
            while (count > 1)
            {
                count--;
                int m = randomGenerator.Next(count + 1);
                T value = list[m];
                list[m] = list[count];
                list[count] = value;
            }
        }
    }
}
