using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Leaf是叶子节点
/// </summary>
namespace CompositePattern
{
    class Leaf : Component
    {
        public Leaf(string componentName) : base(componentName)
        {
        }

        public override void Add(Component component)
        {
            Console.WriteLine("叶子节点不能添加新的子节点！");
        }

        public override void Remove(Component compontent)
        {
            Console.WriteLine("叶子节点不能移除子节点！");
        }

        public override void Show(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write("+");
            }
            Console.WriteLine(componentName + "\n");
        }
    }
}
