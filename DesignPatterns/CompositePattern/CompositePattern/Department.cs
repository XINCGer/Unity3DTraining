using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 部门类
/// </summary>
namespace CompositePattern
{
    /// <summary>
    /// 整个学校算是一个部门，单个学院甚至单个办公司也算是一个部门
    /// </summary>
    public abstract class Department
    {
        protected string name;
        public Department(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 增加部门
        /// </summary>
        /// <param name="department"></param>
        public abstract void Add(Department department);

        /// <summary>
        /// 移除部门
        /// </summary>
        /// <param name="department"></param>
        public abstract void Remove(Department department);

        /// <summary>
        /// 展示部门结构
        /// </summary>
        /// <param name="depaerment"></param>
        public abstract void Show(int depth);

        /// <summary>
        /// 部门汇报工作
        /// </summary>
        public abstract void ReportWork();
    }
}
