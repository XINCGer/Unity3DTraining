using System;

namespace BuilderPattern
{
    /// <summary>
    /// 学生A会做通入少量二氧化碳的实验
    /// </summary>
    class StudentA : Student
    {
        public override void PourCarbon()
        {
            Console.WriteLine("学生A准备好了实验器材");
        }

        public override void PourReagent()
        {
            Console.WriteLine("学生A加入氢氧化钡");
        }

        public override void PrePareEx()
        {
            Console.WriteLine("学生A通入少量的二氧化碳");
        }

        public override void ShowResult()
        {
            Console.WriteLine("出现了沉淀");
        }
    }
}
