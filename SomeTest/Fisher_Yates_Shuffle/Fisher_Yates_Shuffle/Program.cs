using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/* shuffle的意思就是让序列乱序，本质上就是让序列里面的每一个元素等概率的重新分布在序列的任何位置。
在使用MP3听歌（是不是暴露的年龄）的时候，就有两个功能：shuffle，random，二者的区别在于，前者打
乱播放顺序，保证所有的歌曲都会播放一遍；而后者每次随机选择一首。 
                            WIKI: https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
 */

/*  Fisher–Yates shuffle算法伪码如下：
 *  
    -- To shuffle an array a of n elements(indices 0..n-1):
    for i from n−1 downto 1 do
    j ← random integer such that 0 ≤ j ≤ i
    exchange a[j] and a[i]

    第一步 即从0到N-1个元素中随机选择一个与第N-1个替换
    
    第二步 从0到N-2个元素中随机选择一个与第N-2个替换

    第k步 从0到N-k个元素中随机选择一个与第N-K个替换

    要证明算法的正确性也很简单，即任何一个元素shuffle之后出现在任意位置的概率都是1/N。任意一个元素，放在第N-1个位置的概率是1/N，
    放在pos N-2的位置是 (N-1)/N * 1 / (N-1) = 1/N 。需要注意的是，一个元素一旦被交换到了序列的尾部，那么就不会再被选中，
    这也是算法一目了然的原因。

　　上面的实现是从后到前的，当然也可以从前到后，即先从0到N-1个元素中随机选择一个与第0个交换，然后从1到N-1个元素中随机选择一个与第1个
    交换 。。。只不过写代码的时候会稍微麻烦一点点，wiki上也有相应的伪码。
*/
namespace Fisher_Yates_Shuffle
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> numList = new List<int>() { 1, 2, 3, 4, 5 };
            List<string> stringList = new List<string>() { "A", "B", "C", "D", "E" };

            numList.ShuffleWithWhileLoop();
            foreach (var tmp in numList)
            {
                Console.WriteLine(tmp);
            }
            numList.ShuffleWithForLoop();
            foreach (var tmp in numList)
            {
                Console.WriteLine(tmp);
            }

            stringList.ShuffleWithWhileLoop();
            foreach (var tmp in stringList)
            {
                Console.WriteLine(tmp);
            }
            stringList.ShuffleWithForLoop();
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
        public static void ShuffleWithWhileLoop<T>(this IList<T> list)
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

        public static void ShuffleWithForLoop<T>(this IList<T> list)
        {
            Random randomGenerator = new Random();
            for (int i = list.Count-1; i > 0; i--)
            {
                int m = randomGenerator.Next(i+1);
                T value = list[m];
                list[m] = list[i];
                list[i] = value;
            }
        }
    }
}
