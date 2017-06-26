using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositePattern
{
    /// <summary>
    /// 学生管理处相当于叶子节点
    /// </summary>
    class StudentOffice : Department
    {
        public StudentOffice(string name) : base(name)
        {
        }

        public override void Add(Department department)
        {
        }

        public override void Remove(Department department)
        {
        }

        public override void ReportWork()
        {
            Console.WriteLine(name+"汇报了学生工作！");
        }

        public override void Show(int depth)
        {
            for(int i = 0; i > depth; i++)
            {
                Console.Write("+");
            }
            Console.WriteLine(name);
        }
    }
}
