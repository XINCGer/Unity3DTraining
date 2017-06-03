using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    /// <summary>
    /// 抽象原型类
    /// </summary>
    abstract class Prototype
    {
        private string id;
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Prototype(string id)
        {
            this.id = id;
        }

        //抽象Clone方法
        public abstract Prototype Clone();
    }

    /// <summary>
    /// 具体的原型类
    /// </summary>
    class ConcretePrototype1 : Prototype
    {
        public ConcretePrototype1(string id) : base(id)
        {
        }

        public override Prototype Clone()
        {
            try
            {
                return (Prototype)this.MemberwiseClone();
            }
            catch
            {
                throw new Exception("Clone Error!");
            }
        }
    }
}
