using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Composite类是分支节点的类
/// </summary>
namespace CompositePattern
{
    class Composite : Component
    {
        private List<Component> childList = new List<Component>();

        public Composite(string componentName) : base(componentName)
        {
        }

        public override void Add(Component component)
        {
            childList.Add(component);
        }

        public override void Remove(Component compontent)
        {
            childList.Remove(compontent);
        }

        public override void Show(int depth)
        {
            for(int i = 0; i < depth; i++)
            {
                Console.WriteLine("+");
            }
            Console.WriteLine(componentName + "\n");

        }
    }
}
