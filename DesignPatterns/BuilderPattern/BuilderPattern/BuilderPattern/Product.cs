using System;
using System.Collections.Generic;

namespace BuilderPattern
{
    /// <summary>
    /// 具体的产品类
    /// </summary>
    class Product
    {
        List<string> parts = new List<string>();

        public void Add(string part)
        {
            parts.Add(part);
        }

        public void Show()
        {
            Console.WriteLine("Create Product");
            for (int i = 0; i < parts.Count; i++)
            {
                Console.WriteLine(parts[i]);
            }
        }
    }
}
