using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 组合模式
/// </summary>
namespace CompositePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //以树形结构为例展示组合模式
            Composite root = new Composite("树根");
            root.Add(new Leaf("叶子A"));
            root.Add(new Leaf("叶子B"));

            Composite composite1 = new Composite("节点1");
            composite1.Add(new Leaf("1叶子A"));
            composite1.Add(new Leaf("1叶子B"));

            Composite composite2 = new Composite("节点2");
            composite2.Add(new Leaf("2叶子A"));
            composite2.Add(new Leaf("2叶子B"));

            composite1.Add(composite2);
            root.Add(composite1);
            root.Show(1);
        }
    }
}
