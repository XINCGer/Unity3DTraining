using System;

namespace BuilderPattern
{
    /// <summary>
    /// 学生B会做通入大量二氧化碳的实验
    /// </summary>
    class StudentB : Student
    {
        public override void PourCarbon()
        {
            Console.WriteLine("学生B准备好了实验器材");
        }

        public override void PourReagent()
        {
            Console.WriteLine("学生B加入氢氧化钡");
        }

        public override void PrePareEx()
        {
            Console.WriteLine("学生B通入大量的二氧化碳");
        }

        public override void ShowResult()
        {
            Console.WriteLine("出现沉淀后又消失了");
        }
    }
}
