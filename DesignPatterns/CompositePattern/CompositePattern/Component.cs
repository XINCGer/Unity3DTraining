using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositePattern
{
    /// <summary>
    /// Component类是树中分支节点与叶子节点的抽象父类
    /// </summary>
    abstract class Component
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        protected string componentName;

        public Component(string componentName)
        {
            this.componentName = componentName;
        }

        /// <summary>
        /// 增加分支/叶子节点的方法
        /// </summary>
        /// <param name="component"></param>
        public abstract void Add(Component component);
        /// <summary>
        /// 移除分支/叶子节点方法
        /// </summary>
        /// <param name="compontent"></param>
        public abstract void Remove(Component compontent);
        /// <summary>
        /// 按照深度显示树形结构的方法
        /// </summary>
        /// <param name="depth"></param>
        public abstract void Show(int depth);
    }
}
