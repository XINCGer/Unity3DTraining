using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgePattern
{
    /// <summary>
    /// 系所抽象类代码
    /// </summary>
    abstract class Departments
    {
        //系所包含的课程对象
        protected Math mathCourse;

        /// <summary>
        /// 设置课程
        /// </summary>
        /// <param name="math"></param>
        public void SetCourse(Math math)
        {
            this.mathCourse = math;
        }

        /// <summary>
        /// 选择课程
        /// </summary>
        public abstract void Select();
    }

    //具体的系所代码
    /// <summary>
    /// 计算机系
    /// </summary>
    class Computer : Departments
    {
        public override void Select()
        {
            Console.WriteLine("计算机专业同学选课");
            mathCourse.Select();
        }
    }

    class Mathematics : Departments
    {
        public override void Select()
        {
            Console.WriteLine("数学系的同学选课");
            mathCourse.Select();
        }
    }
}
