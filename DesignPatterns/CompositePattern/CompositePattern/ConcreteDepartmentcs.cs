using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositePattern
{
    /// <summary>
    /// 实体部门相当于分支节点
    /// </summary>
    class ConcreteDepartmentcs : Department
    {
        private List<Department> childList = new List<Department>();

        public ConcreteDepartmentcs(string name) : base(name)
        {
        }

        public override void Add(Department department)
        {
            childList.Add(department);
        }

        public override void Remove(Department department)
        {
            childList.Remove(department);
        }

        public override void ReportWork()
        {
            for(int i = 0; i < childList.Count; i++)
            {
                childList[i].ReportWork();
            }
        }

        public override void Show(int depth)
        {
            for(int i = 0; i < depth; i++)
            {
                Console.Write("+");
            }
            Console.WriteLine(name+"\n");
            for(int i = 0; i < childList.Count; i++)
            {
                childList[i].Show(depth+2);
            }
        }
    }
}
