using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 装饰器类
    /// </summary>
    class Decorator : Component
    {
        /// <summary>
        /// 仅供本类调用的Component对象
        /// </summary>
        protected Component component;

        /// <summary>
        /// 设置Component对象，
        /// </summary>根据里氏代换原则可以传入其子类的对象作为参数
        /// <param name="component"></param>
        public void SetComponent(Component component)
        {
            this.component = component;
        }

        /// <summary>
        /// 重写基本操作
        /// </summary>
        public override void Operation()
        {
            if (null != component)
            {
                component.Operation();
            }
        }
    }

    /// <summary>
    /// 具体的装饰器A
    /// </summary>
    class ConcreteDecoratorA : Decorator
    {
        private string addedState;

        public override void Operation()
        {
            base.Operation();
            addedState = "New State!";
            Console.WriteLine("本类有状态"+addedState);
        }
    }

    /// <summary>
    /// 具体的装饰器类B
    /// </summary>
    class ConcreteDecoratorB : Decorator
    {

        public override void Operation()
        {
            base.Operation();
            AddBehavior();
        }

        private void AddBehavior()
        {
            Console.WriteLine("本类有个新方法！");
        }
    }
}
