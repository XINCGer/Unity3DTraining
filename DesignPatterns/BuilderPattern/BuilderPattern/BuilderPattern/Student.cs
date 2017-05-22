namespace BuilderPattern
{
    /// <summary>
    /// 定义一个抽象的学生基类，使不同的学生类继承该基类，以保证统一的“组件”不会在实现时丢失
    /// 由于抽象类Student定义了一些系列的抽象方法，因为子类如果不实现就报错，这样就避免了丢失组件的情况
    /// </summary>
    public abstract class Student
    {
        /// <summary>
        /// 实验前的准备工作
        /// </summary>
        public abstract void PrePareEx();
        /// <summary>
        /// 加入氢氧化钡
        /// </summary>
        public abstract void PourReagent();
        /// <summary>
        /// 加入二氧化碳
        /// </summary>
        public abstract void PourCarbon();
        /// <summary>
        /// 展示实验结果
        /// </summary>
        public abstract void ShowResult();
    }
}
