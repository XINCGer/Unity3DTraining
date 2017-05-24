using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern
{
    class Teacher
    {
        private Student student;

        public Teacher(Student student)
        {
            this.student = student;
        }

        //老师指导学生实验
        public void DirectExperiment()
        {
            student.PrePareEx();
            student.PourReagent();
            student.PourCarbon();
            student.ShowResult();
        }
    }
}
