using System;

namespace BuilderPattern
{
    /// <summary>
    /// 抽象的建造者类
    /// </summary>
    abstract class Builder
    {
        public abstract void BuilderPartA();
        public abstract void BuilderPartB();

        public abstract Product GetResult();
    }

    /// <summary>
    /// 具体的建造者类A
    /// </summary>
    class ConcreteBuliderA : Builder
    {
        private Product product = new Product();

        public override void BuilderPartA()
        {
            product.Add("Part A");
        }

        public override void BuilderPartB()
        {
            product.Add("Part B");
        }

        public override Product GetResult()
        {
            return product;
        }
    }

    /// <summary>
    /// 具体的建造者类B
    /// </summary>
    class ConcreteBuilderB : Builder
    {
        private Product product = new Product();

        public override void BuilderPartA()
        {
            product.Add("Part W");
        }

        public override void BuilderPartB()
        {
            product.Add("Part Z");
        }

        public override Product GetResult()
        {
            return product;
        }
    }


    /// <summary>
    /// 指挥者类
    /// </summary>
    class Director
    {
        /// <summary>
        /// 构建函数
        /// </summary>
        /// <param name="builder"></param>建造者
        public void Construct(Builder builder)
        {
            builder.BuilderPartA();
            builder.BuilderPartB();
        }
    }
}
